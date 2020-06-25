using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Prism.Commands;
using Prism.Mvvm;
using WeMusic.Control;
using WeMusic.Model;
using WeMusic.Model.Player;

namespace WeMusic.ViewModel
{
    public class LyricPageViewModel : BindableBase
    {
        //样式常量
        public const int LINE_HEIGHT = 40;
        public const int CURRENT_LINE_HEIGHT = 60;
        public const int FONT_SIZE = 18;
        public const int CURRENT_FONT_SIZE = 25;

        private Border blankFront = new Border();
        private Border blankBack = new Border();
        private static DispatcherTimer timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(0.1) };
        public int CurrentIndex { get; set; } = 0;
        private List<LyricItem> lyrics;

        private string _musicName = "微音乐";
        public string MusicName
        {
            get { return _musicName; }
            set
            {
                _musicName = value;
                this.RaisePropertyChanged("MusicName");
            }
        }

        private string _musicArtist = "微音乐";
        public string MusicArtist
        {
            get { return _musicArtist; }
            set
            {
                _musicArtist = value;
                this.RaisePropertyChanged("MusicArtist");
            }
        }

        private string _album = "无";
        public string Album
        {
            get { return _album; }
            set
            {
                _album = value;
                this.RaisePropertyChanged("Album");
            }
        }

        private Uri _cover = new Uri("pack://application:,,,/Resources/DefaultCover.png");
        public Uri Cover
        {
            get { return _cover; }
            set
            {
                _cover = value;
                this.RaisePropertyChanged("Cover");
            }
        }

        public LyricPageViewModel()
        {
            ViewModelManager.LyricPageViewModel = this;
            LrcContent.Children.Add(blankFront);
            LrcContent.Children.Add(blankBack);
            BlankHeightUpdateCommand = new DelegateCommand(new Action(BlankHeightUpdateExecute));
        }

        private StackPanel _lrcContent = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center, Orientation = Orientation.Vertical };
        public StackPanel LrcContent
        {
            get { return _lrcContent; }
            set
            {
                _lrcContent = value;
                this.RaisePropertyChanged("LrcContent");
            }
        }

        public DelegateCommand BlankHeightUpdateCommand { get; set; }

        public void Init(List<LyricItem> lyrics)
        {
            //清除面板内容
            LrcContent.Children.Clear();
            LrcContent.Children.Add(blankFront);
            LrcContent.Children.Add(blankBack);
            //检测线程更新
            timer?.Stop();
            timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(0.1) };

            BlankHeightUpdateExecute();
            this.lyrics = lyrics;
            for (int i = 0; i < lyrics.Count; i++)
            {
                Grid grid = new Grid { Height = LINE_HEIGHT };
                grid.Children.Add(new TextBlock
                {
                    Text = lyrics[i].Text,
                    FontSize = FONT_SIZE,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
                LrcContent.Children.Insert(i + 1, grid);
            }
            timer.Tick += TimeTick;
            CurrentIndex = 0;
            MusicName = PlayerManager.PlayMusic.Name;
            MusicArtist = PlayerManager.PlayMusic.Artists;
            Album = PlayerManager.PlayMusic.Album;
            if (System.IO.File.Exists(DownloadManager.CoverCachePath + PlayerManager.PlayMusic.Id + ".jpg"))
            {
                Cover = new Uri($"pack://siteoforigin:,,,/Cache/Cover/{PlayerManager.PlayMusic.Id}.jpg", UriKind.Absolute);
            }
            timer.Start();
        }

        public void BlankHeightUpdateExecute()
        {
            if (PageManager.LyricPage.LrcView.ActualHeight / 2 - LINE_HEIGHT / 2 > 0)
            {
                blankBack.Height = PageManager.LyricPage.LrcView.ActualHeight / 2 - LINE_HEIGHT / 2;
                blankFront.Height = PageManager.LyricPage.LrcView.ActualHeight / 2 - LINE_HEIGHT / 2;
            }
        }

        private void TimeTick(object sender, EventArgs e)
        {
            //歌曲暂停或结束，歌词滚动停止
            if (PlayerManager.State != NAudio.Wave.PlaybackState.Playing) { timer.Stop(); return; }

            int seconds = (int)PlayerManager.Position.TotalSeconds;
            if (CurrentIndex < 0) { return; }
            if (CurrentIndex >= lyrics.Count - 1) 
            {
                //当前为最后一句歌词，直接返回，若歌词进度被改变，继续歌词滚动事件
                if (seconds>=lyrics[CurrentIndex].Time  ) { return; }
                CurrentIndex--;
            }

            
            //当前歌词无需滚动
            if (seconds > lyrics[CurrentIndex].Time && seconds < lyrics[CurrentIndex + 1].Time) { return; }

            //前一段歌词样式恢复
            Grid lastGrid = LrcContent.Children[CurrentIndex + 1] as Grid;
            lastGrid.Height = LINE_HEIGHT;
            (lastGrid.Children[0] as TextBlock).Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            (lastGrid.Children[0] as TextBlock).FontSize = FONT_SIZE;

            //获取当前进度的歌词
            int nextIndex = 0;
            //判断是否最后一句歌词
            if (seconds >= lyrics[lyrics.Count - 1].Time)
            {
                nextIndex = lyrics.Count - 1;
            }
            //判断是否下一句歌词
            else if (seconds >= lyrics[CurrentIndex + 1].Time && seconds < lyrics[CurrentIndex + 2].Time)
            {
                nextIndex = CurrentIndex + 1;
            }
            //歌词进度改变了，通过遍历获取当前歌词
            else
            {
                for (int i = lyrics.Count - 1; i >= 0; i--)
                {
                    if (seconds >= lyrics[i].Time)
                    {
                        nextIndex = i;
                        break;
                    }
                }
            }

            //当前歌词样式设置
            Grid currentGrid = LrcContent.Children[nextIndex + 1] as Grid;
            currentGrid.Height = CURRENT_LINE_HEIGHT;
            (currentGrid.Children[0] as TextBlock).Foreground = new LinearGradientBrush(Color.FromRgb(79, 172, 254), Color.FromRgb(0, 242, 254), 0);
            (currentGrid.Children[0] as TextBlock).FontSize = CURRENT_FONT_SIZE;

            //动画设置
            DoubleAnimation verticalAnimation = new DoubleAnimation
            {
                From = PageManager.LyricPage.LrcView.VerticalOffset,
                To = LINE_HEIGHT * (nextIndex + 1) - CURRENT_LINE_HEIGHT / 2,
                Duration = new Duration(TimeSpan.FromMilliseconds(500)),
                EasingFunction = new SineEase()
            };
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(verticalAnimation);
            Storyboard.SetTarget(verticalAnimation, PageManager.LyricPage.LrcView);
            Storyboard.SetTargetProperty(verticalAnimation, new PropertyPath(ScrollViewerHelper.VerticalOffsetProperty));
            storyboard.Begin();

            //设置当前序号
            CurrentIndex = nextIndex;
        }

    }
}
