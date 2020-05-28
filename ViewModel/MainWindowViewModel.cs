using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Prism.Commands;
using Prism.Mvvm;
using WeMusic.Model;
using WeMusic.Model.Player;
using WeMusic.Model.DbModel;
using SqlSugar;
using System.Collections.ObjectModel;
using WeMusic.Interface;
using WeMusic.Enum;

namespace WeMusic.ViewModel
{
    public sealed class MainWindowViewModel : BindableBase
    {
        private const string SHOW_LRC_ICON = "pack://application:,,,/Resources/ShowLrcIcon.png";
        private const string HIDDEN_LRC_ICON = "pack://application:,,,/Resources/HiddenLrcIcon.png";

        public MainWindowViewModel()
        {
            ViewModelManager.MainWindowViewModel = this;
            var historys = new SearchHistoryManager().GetList();
            if (historys.Count <= 10)
            {
                SearchHistoryList = new ObservableCollection<SearchHistoryModel>();
                historys.ForEach(item => { SearchHistoryList.Insert(0, item); });
            }
            else
            {
                SearchHistoryList = new ObservableCollection<SearchHistoryModel>();
                historys.GetRange(historys.Count - 10, 10).ForEach(item => { SearchHistoryList.Insert(0, item); });
            }

            //Init();
            ClickMinimizedCommand = new DelegateCommand(new Action(() => State = WindowState.Minimized));
            ClickClosedCommand = new DelegateCommand(new Action(CloseWindow));
            ClickMaximizedCommand = new DelegateCommand(new Action(ClickMaximizedExecute));
            ClickSearchCommand = new DelegateCommand(new Action(ClickSearchExecute));
            ClickPlayCommand = new DelegateCommand(new Action(ClickPlayExecute));
            ClickPauseCommand = new DelegateCommand(new Action(ClickPauseExecute));
            VolumeChangedCommand = new DelegateCommand<object>(new Action<object>((parameter) => PlayerManager.Volume = Volume));
            MusicPositionChangedCommand = new DelegateCommand<object>(new Action<object>(MusicPositionChangedExecute));
            MusicPositionBeginChangedCommand = new DelegateCommand(new Action(() => PlayerNotification.Stop()));
            DragMoveCommand = new DelegateCommand(new Action(() => Application.Current.MainWindow.DragMove()));
            MouseMovedCoverCommand = new DelegateCommand(new Action(MouseMovedCoverExecute));
            MouseLeftCoverCommand = new DelegateCommand(new Action(() => HiddenCoverVisibility = Visibility.Hidden));
            CoverClickCommand = new DelegateCommand(new Action(CoverClickExecute));
            ClickPreviousMusicCommand = new DelegateCommand(new Action(ClickPreviousMusicExecute));
            ClickNextMusicCommand = new DelegateCommand(new Action(ClickNextMusicExecute));
            PlayModeIcon = Application.Current.FindResource("PlaySingle") as Geometry;
            ChangePlayModeCommand = new DelegateCommand<object>(new Action<object>(ChangePlayModeExecute));
            ClickSearchHistoryCommand = new DelegateCommand<object>(new Action<object>(ClickSearchHistoryExecute));
        }

        private WindowState _state = WindowState.Normal;
        public WindowState State
        {
            get { return _state; }
            set
            {
                _state = value;
                this.RaisePropertyChanged("State");
            }
        }

        private Visibility _maxButtonVisibility = Visibility.Visible;
        public Visibility MaxButtonVisibility
        {
            get { return _maxButtonVisibility; }
            set
            {
                _maxButtonVisibility = value;
                this.RaisePropertyChanged("MaxButtonVisibility");
            }
        }

        private Visibility _returnButtonVisibility = Visibility.Hidden;
        public Visibility ReturnButtonVisibility
        {
            get { return _returnButtonVisibility; }
            set
            {
                _returnButtonVisibility = value;
                this.RaisePropertyChanged("ReturnButtonVisibility");
            }
        }

        private Thickness _gridMargin = new Thickness(0, 0, 0, 0);
        public Thickness GridMargin
        {
            get { return _gridMargin; }
            set
            {
                _gridMargin = value;
                this.RaisePropertyChanged("GridMargin");
            }
        }

        private string _searchContent = string.Empty;
        public string SearchContent
        {
            get { return _searchContent; }
            set
            {
                _searchContent = value;
                this.RaisePropertyChanged("SearchContent");
            }
        }

        private Page _currentPage = PageManager.BasePage;
        public Page CurrentPage
        {
            get { return PageManager.CurrentPage; }
            set
            {
                PageManager.CurrentPage = value;
                this.RaisePropertyChanged("CurrentPage");
                //歌词背景切换
                if (value == PageManager.LyricPage)
                {
                    try
                    {
                        if (PlayerManager.PlayMusic != null)
                        {
                            Background = new Uri($"pack://siteoforigin:,,,/Cache/Cover/{PlayerManager.PlayMusic.Id}.jpg", UriKind.Absolute);
                        }
                    }
                    catch { }
                }
                else
                {
                    Background = null;
                }

            }
        }

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

        private string _musicConnection = " - ";
        public string MusicConnection
        {
            get { return _musicConnection; }
            set
            {
                _musicConnection = value;
                this.RaisePropertyChanged("MusicConnection");
            }
        }

        private TimeSpan _musicMaxTime = new TimeSpan(1);
        public TimeSpan MusicMaxTime
        {
            get { return _musicMaxTime; }
            set
            {
                _musicMaxTime = value;
                this.RaisePropertyChanged("MusicMaxTime");
            }
        }

        private TimeSpan _musicNowTime = new TimeSpan(0);
        public TimeSpan MusicNowTime
        {
            get { return _musicNowTime; }
            set
            {
                _musicNowTime = value;
                this.RaisePropertyChanged("MusicNowTime");
            }
        }

        private double _volume = PlayerManager.Volume;
        public double Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;
                this.RaisePropertyChanged("Volume");
            }
        }

        private Uri _musicSource;
        public Uri MusicSource
        {
            get { return _musicSource; }
            set
            {
                _musicSource = value;
                this.RaisePropertyChanged("MusicSource");
            }
        }

        private Visibility _playButtonVisibility = Visibility.Visible;
        public Visibility PlayButtonVisibility
        {
            get { return _playButtonVisibility; }
            set
            {
                _playButtonVisibility = value;
                this.RaisePropertyChanged("PlayButtonVisibility");
            }
        }

        private Visibility _pauseButtonVisibility = Visibility.Hidden;
        public Visibility PauseButtonVisibility
        {
            get { return _pauseButtonVisibility; }
            set
            {
                _pauseButtonVisibility = value;
                this.RaisePropertyChanged("PauseButtonVisibility");
            }
        }

        private Uri _coverSource = new Uri("/Resources/DefaultCover.png", UriKind.Relative);
        public Uri CoverSource
        {
            get { return _coverSource; }
            set
            {
                _coverSource = value;
                this.RaisePropertyChanged("CoverSource");
            }
        }

        private Uri _hiddenCoverSource = new Uri(SHOW_LRC_ICON);
        public Uri HiddenCoverSource
        {
            get { return _hiddenCoverSource; }
            set
            {
                _hiddenCoverSource = value;
                this.RaisePropertyChanged("HiddenCoverSource");
            }
        }

        private Visibility _hiddenCoverVisibility = Visibility.Hidden;
        public Visibility HiddenCoverVisibility
        {
            get { return _hiddenCoverVisibility; }
            set
            {
                _hiddenCoverVisibility = value;
                this.RaisePropertyChanged("HiddenCoverVisibility");
            }
        }

        private ObservableCollection<IMusic> _showMusicList;
        public ObservableCollection<IMusic> ShowMusicList
        {
            get { return _showMusicList; }
            set
            {
                _showMusicList = value;
                this.RaisePropertyChanged("ShowMusicList");
            }
        }

        private Uri _background;
        public Uri Background
        {
            get { return _background; }
            set
            {
                _background = value;
                this.RaisePropertyChanged("Background");
            }
        }

        private PointCollection _points = new PointCollection();
        public PointCollection Points
        {
            get { return _points; }
            set
            {
                _points = value;
                this.RaisePropertyChanged("Points");
            }
        }

        private string _musicListTitle;
        public string MusicListTitle
        {
            get { return _musicListTitle; }
            set
            {
                _musicListTitle = value;
                this.RaisePropertyChanged("MusicListTitle");
            }
        }

        private Geometry _playModeIcon;
        public Geometry PlayModeIcon
        {
            get { return _playModeIcon; }
            set
            {
                _playModeIcon = value;
                this.RaisePropertyChanged("PlayModeIcon");
            }
        }

        private ObservableCollection<SearchHistoryModel> _searchHistoryList = new ObservableCollection<SearchHistoryModel>();
        public ObservableCollection<SearchHistoryModel> SearchHistoryList
        {
            get { return _searchHistoryList; }
            set
            {
                _searchHistoryList = value;
                this.RaisePropertyChanged("SearchHistoryList");
            }
        }

        private bool _searchHistoryIsOpen = false;
        public bool SearchHistoryIsOpen
        {
            get { return _searchHistoryIsOpen; }
            set
            {
                _searchHistoryIsOpen = value;
                this.RaisePropertyChanged("SearchHistoryIsOpen");
            }
        }

        public DelegateCommand ClickMinimizedCommand { get; set; }
        public DelegateCommand ClickMaximizedCommand { get; set; }
        public DelegateCommand ClickClosedCommand { get; set; }
        public DelegateCommand ClickPlayCommand { get; set; }
        public DelegateCommand ClickPauseCommand { get; set; }
        public DelegateCommand ClickSearchCommand { get; set; }
        public DelegateCommand MediaOpenedCommand { get; set; }
        public DelegateCommand<object> VolumeChangedCommand { get; set; }
        public DelegateCommand<object> MusicPositionChangedCommand { get; set; }
        public DelegateCommand MusicPositionBeginChangedCommand { get; set; }
        public DelegateCommand DragMoveCommand { get; set; }
        public DelegateCommand MouseMovedCoverCommand { get; set; }
        public DelegateCommand MouseLeftCoverCommand { get; set; }
        public DelegateCommand CoverClickCommand { get; set; }
        public DelegateCommand ClickPreviousMusicCommand { get; set; }
        public DelegateCommand ClickNextMusicCommand { get; set; }
        public DelegateCommand<object> ChangePlayModeCommand { get; set; }
        public DelegateCommand<object> ClickSearchHistoryCommand { get; set; }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        public void ClickMaximizedExecute()
        {
            switch (State)
            {
                case WindowState.Maximized:
                    State = WindowState.Normal;
                    MaxButtonVisibility = Visibility.Visible;
                    ReturnButtonVisibility = Visibility.Hidden;
                    GridMargin = new Thickness(0, 0, 0, 0);
                    break;
                case WindowState.Normal:
                    State = WindowState.Maximized;
                    MaxButtonVisibility = Visibility.Hidden;
                    ReturnButtonVisibility = Visibility.Visible;
                    GridMargin = new Thickness(8, 8, 8, 8);
                    break;

            }
        }

        /// <summary>
        /// 点击播放
        /// </summary>
        public void ClickPlayExecute()
        {
            if (PlayerManager.PlayMusic == null) { return; }
            PlayerManager.Play();
        }

        /// <summary>
        /// 点击暂停
        /// </summary>
        public void ClickPauseExecute()
        {
            PlayerManager.Pause();
            PlayButtonVisibility = Visibility.Visible;
            PauseButtonVisibility = Visibility.Hidden;
        }

        /// <summary>
        /// 搜索
        /// </summary>
        public void ClickSearchExecute()
        {
            SearchHistoryIsOpen = false;
            var shm = new SearchHistoryModel(SearchContent);
            new SearchHistoryManager().Insert(shm);
            //如果搜索历史列表项大于等于10个，移出最后一个
            if (SearchHistoryList.Count >= 10) { SearchHistoryList.RemoveAt(9); }
            SearchHistoryList.Insert(0, shm);
            CurrentPage = PageManager.SearchPage;
            (PageManager.SearchPage.DataContext as SearchPageViewModel).SearchContent = SearchContent;
        }

        /// <summary>
        /// 音乐进度改变
        /// </summary>
        /// <param name="parameter"></param>
        public void MusicPositionChangedExecute(object parameter)
        {
            if (PlayerManager.State == NAudio.Wave.PlaybackState.Stopped) { return; }
            PlayerManager.Position = TimeSpan.FromSeconds((double)parameter);
            PlayerNotification.Start();
        }

        /// <summary>
        /// 设置缓冲状态
        /// </summary>
        public void SetBufferState()
        {
            MusicName = "加载中...";
            MusicArtist = "";
            MusicConnection = "";
            MusicNowTime = new TimeSpan(0);
            MusicMaxTime = new TimeSpan(1);
        }

        /// <summary>
        /// 设置播放状态
        /// </summary>
        public void SetPlayState()
        {
            MusicName = PlayerManager.PlayMusic.Name;
            MusicArtist = PlayerManager.PlayMusic.Artists;
            MusicConnection = " - ";
            PlayButtonVisibility = Visibility.Hidden;
            PauseButtonVisibility = Visibility.Visible;
        }

        /// <summary>
        /// 设置停止播放状态
        /// </summary>
        public void SetStopState()
        {
            MusicName = "微音乐";
            MusicArtist = "微音乐";
            MusicConnection = " - ";
            PlayButtonVisibility = Visibility.Visible;
            PauseButtonVisibility = Visibility.Hidden;
            CoverSource = new Uri("/Resources/DefaultCover.png", UriKind.Relative);
        }

        /// <summary>
        /// 鼠标移出封面
        /// </summary>
        public void MouseMovedCoverExecute()
        {
            HiddenCoverVisibility = Visibility.Visible;
            if (CurrentPage != PageManager.LyricPage)
            {
                HiddenCoverSource = new Uri(SHOW_LRC_ICON);
            }
            else
            {
                HiddenCoverSource = new Uri(HIDDEN_LRC_ICON);
            }
        }

        /// <summary>
        /// 点击封面
        /// </summary>
        public void CoverClickExecute()
        {
            if (CurrentPage != PageManager.LyricPage)
            {
                CurrentPage = PageManager.LyricPage;
                if (PlayerManager.Lyric == null) { return; }
                //歌词初始化
                ViewModelManager.LyricPageViewModel.Init(PlayerManager.Lyric);
            }
            else
            {
                DoubleAnimation faded = new DoubleAnimation(1, 0, new TimeSpan(0, 0, 0, 0, 200));
                ThicknessAnimation upper = new ThicknessAnimation(new Thickness(), new Thickness(0, 150, 0, -150), new TimeSpan(0, 0, 0, 0,200));
                faded.Completed += new EventHandler((o, e) => PageManager.LyricPage.NavigationService?.GoBack());
                faded.DecelerationRatio = 1;
                PageManager.LyricPage.BeginAnimation(Page.OpacityProperty, faded);
                PageManager.LyricPage.BeginAnimation(Page.MarginProperty, upper);
            }
        }

        /// <summary>
        /// 设置主窗口背景
        /// </summary>
        /// <param name="type">0=null;1=当前播放音乐的背景;</param>
        public void SetBackground(int type)
        {
            try
            {
                if (type == 0) { Background = null;return; }
                if (PlayerManager.PlayMusic != null)
                {
                    Background = new Uri($"pack://siteoforigin:,,,/Cache/Cover/{PlayerManager.PlayMusic.Id}.jpg", UriKind.Absolute);
                }
            }
            catch { }
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public void CloseWindow()
        {
            PlayerManager.Stop();
            Application.Current.MainWindow.Close();
        }

        /// <summary>
        /// 点击上一首歌
        /// </summary>
        public void ClickPreviousMusicExecute()
        {
            PlayerManager.Stop();
            try
            {
                PlayerManager.PlayMusic = PlayerList.Previous();
                PlayerManager.Play();
            }
            catch (Exception)
            {
                PlayerManager.Stop();
            }
        }

        /// <summary>
        /// 点击下一首歌
        /// </summary>
        public void ClickNextMusicExecute()
        {
            PlayerManager.Stop();
            try
            {
                PlayerManager.PlayMusic = PlayerList.Next();
                PlayerManager.Play();
            }
            catch (Exception)
            {
                PlayerManager.Stop();
            }
        }

        /// <summary>
        /// 改变播放模式
        /// </summary>
        /// <param name="parameter"></param>
        public void ChangePlayModeExecute(object parameter)
        {
            int.TryParse(parameter.ToString(), out int mode);
            switch (mode)
            {
                case (int)PlayMode.ListLoop:
                    PlayModeIcon= Application.Current.FindResource("PlayLoop") as Geometry;
                    PlayerList.Mode = PlayMode.ListLoop;
                    break;
                case (int)PlayMode.RandomLoop:
                    PlayModeIcon = Application.Current.FindResource("PlayRandom") as Geometry;
                    PlayerList.Mode = PlayMode.RandomLoop;
                    break;
                case (int)PlayMode.SingleLoop:
                    PlayModeIcon = Application.Current.FindResource("PlaySingleLoop") as Geometry;
                    PlayerList.Mode = PlayMode.SingleLoop;
                    break;
                case (int)PlayMode.SinglePlay:
                    PlayModeIcon = Application.Current.FindResource("PlaySingle") as Geometry;
                    PlayerList.Mode = PlayMode.SinglePlay;
                    break;
            }
        }

        /// <summary>
        /// 点击搜索列表
        /// </summary>
        /// <param name="parameter"></param>
        public void ClickSearchHistoryExecute(object parameter)
        {
            SearchContent = parameter?.ToString();
            ClickSearchExecute();
        }
    }
}
