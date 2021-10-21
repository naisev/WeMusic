using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using NAudio;
using NAudio.Wave;
using System.Net;
using WeMusic.Interface;
using NAudio.Wave.SampleProviders;
using System.Windows.Media;
using System.IO;
using System.Threading.Tasks;

namespace WeMusic.Model.Player
{
    public static class PlayerManager
    {

        static PlayerManager()
        {
            //player.MediaOpened += Player_MediaOpened;
        }

        private static IWavePlayer player = new WaveOutEvent();
        private static MediaFoundationReader reader = null;
        private static SampleAggregator aggregator = null;
        private static SampleChannel channel = null;

        public static string Source { get; set; }
        public static TimeSpan Length { get { return reader.TotalTime; } }
        public static TimeSpan Position
        {
            /*get { return player.Position.TotalSeconds; }
            set { player.Position = TimeSpan.FromSeconds(value); }*/
            get { return reader.CurrentTime; }
            set { reader.CurrentTime = value; }
        }
        public static PlaybackState State
        {
            get { return player.PlaybackState; }
        }
        public static double Volume
        {
            get { return player.Volume; }
            set { player.Volume = (float)value; }
        }
        public static IMusic PlayMusic { get; set; }
        public static List<LyricItem> Lyric { get; set; }

        public static async void Play()
        {
            //主页面设置缓冲中状态
            ViewModelManager.MainWindowViewModel.SetBufferState();
            //异步加载音乐
            await Task.Run(new Action(() =>
            {
                PrePlay();
            }));
            //主页面设置播放状态
            ViewModelManager.MainWindowViewModel.SetPlayState();

            try
            {
                if (State == PlaybackState.Stopped)
                {
                    reader = new MediaFoundationReader(Source);
                    channel = new SampleChannel(reader);
                    aggregator = new SampleAggregator(channel);
                    aggregator.NotificationCount = reader.WaveFormat.SampleRate / 100;
                    aggregator.PerformFFT = true;
                    aggregator.FftCalculated += DrawFFT;
                    player.Init(aggregator);
                }

                player.Play();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }


            //当前进度设置
            PlayerNotification.Start();

            if (!File.Exists(DownloadManager.MusicCachePath + PlayMusic.Id + ".tmp") && PlayMusic.Origin != Enum.MusicSource.Local)
            {
                DownloadManager.DownloadFileAsync(Source, DownloadManager.MusicCachePath, PlayMusic.Id + ".tmp");
            }
        }

        public static void Play(string url)
        {
            Source = url;
            try
            {
                if (State == PlaybackState.Stopped)
                {
                    reader = new MediaFoundationReader(Source);
                    if (player.PlaybackState != PlaybackState.Stopped) { player.Dispose(); }
                    player.Init(reader);
                }
                player.Play();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public static void Pause()
        {
            if (player == null) { return; }
            player.Pause();
        }

        private static void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            /*var vm = Application.Current.MainWindow.DataContext as MainWindowViewModel;
            vm.MusicMaxTime = player.NaturalDuration.TimeSpan;
            //媒体文件打开成功
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.2);
            timer.Tick += new EventHandler(TimerTick);
            timer.Start();*/
        }

        public static void Stop()
        {
            PlayerNotification.Stop();
            player?.Stop();
            reader?.Dispose();
            player?.Dispose();
            ViewModelManager.MainWindowViewModel.SetStopState();
        }

        public static void DrawFFT(object sender, FftEventArgs e)
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ViewModelManager.MainWindowViewModel.Points = new PointCollection();
                    for (int i = 0; i <= e.Result.Length / 4; i++)
                    {
                        double intensityDB = 10 * Math.Log10(Math.Sqrt(e.Result[i].X * e.Result[i].X + e.Result[i].Y * e.Result[i].Y));
                        double minDB = -90;
                        if (intensityDB < minDB) intensityDB = minDB;
                        double percent = intensityDB / minDB;
                        double y = 100 * (1 - percent);
                        if (i < e.Result.Length / 8)
                        {
                            y *= (double)i * 8 / e.Result.Length;
                        }
                        else
                        {
                            y *= 2 - (double)i * 8 / e.Result.Length;
                        }
                        ViewModelManager.MainWindowViewModel.Points.Add(new Point(i * 4, 68 - y));
                    }
                    ViewModelManager.MainWindowViewModel.Points.Add(new Point(1024, 68));
                    ViewModelManager.MainWindowViewModel.Points.Add(new Point(0, 68));

                });
            });
           

        }

        private static void PrePlay()
        {
            //音乐缓存如果存在，从本地播放
            if (File.Exists(DownloadManager.MusicCachePath + PlayMusic.Id + ".tmp"))
            {
                Source = DownloadManager.MusicCachePath + PlayMusic.Id + ".tmp";
            }
            else
            {
                Source = (PlayMusic as IApi).GetMusicUrl();
            }

            //歌词缓存检测
            if (File.Exists(DownloadManager.LyricCachePath + PlayMusic.Id + ".lrc"))
            {
                Lyric = LyricItem.Parse(File.ReadAllText(DownloadManager.LyricCachePath + PlayMusic.Id + ".lrc"));
            }
            else
            {
                string lyric = (PlayMusic as IApi).GetLyric();
                DownloadManager.SaveFile(Encoding.UTF8.GetBytes(lyric), DownloadManager.LyricCachePath, PlayMusic.Id + ".lrc");
                Lyric = LyricItem.Parse(lyric);
            }

            //专辑封面缓存检测
            if (PlayMusic.Origin == Enum.MusicSource.Local)
            {
                try
                {
                    Properties.Resources.DefaultCover.Save(DownloadManager.CoverCachePath + PlayMusic.Id + ".jpg");
                }
                catch
                {
                    //忽略
                }
            }
            if (!File.Exists(DownloadManager.CoverCachePath + PlayMusic.Id + ".jpg"))
            {
                //异步下载图片
                DownloadManager.DownloadFileAsync((PlayMusic as IApi).GetCoverUrl(), DownloadManager.CoverCachePath, PlayMusic.Id + ".jpg", null,
                    new Action<object, int>((sender, e) =>
                    {
                        if (PageManager.CurrentPage == PageManager.LyricPage)
                        {
                            ViewModelManager.MainWindowViewModel.SetBackground(1);
                            ViewModelManager.LyricPageViewModel.Cover = new Uri($"pack://siteoforigin:,,,/Cache/Cover/{PlayMusic.Id}.jpg", UriKind.Absolute);
                        }
                        ViewModelManager.MainWindowViewModel.CoverSource = new Uri($"pack://siteoforigin:,,,/Cache/Cover/{PlayMusic.Id}.jpg", UriKind.Absolute);
                    }));
            }
            else
            {
                if (PageManager.CurrentPage == PageManager.LyricPage)
                {
                    ViewModelManager.MainWindowViewModel.SetBackground(1);
                }
                ViewModelManager.MainWindowViewModel.CoverSource = new Uri($"pack://siteoforigin:,,,/Cache/Cover/{PlayMusic.Id}.jpg", UriKind.Absolute);
            }


            //如果当前页面是歌词页面，重新加载歌词页面
            if (PageManager.CurrentPage == PageManager.LyricPage)
            {
                //直接加载会抛异常，需要使用Dispatcher.Invoke
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    ViewModelManager.LyricPageViewModel.Init(Lyric);
                }));
            }
        }
    }
}
