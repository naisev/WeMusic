using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Prism.Commands;
using Prism.Mvvm;
using WeMusic.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using WeMusic.Interface;
using System.IO;
using Native.Tool.IniConfig;
using WeMusic.Enum;
using WeMusic.Model.Player;
using WeMusic.Model.DbModel;

namespace WeMusic.ViewModel
{
    public class SearchPageViewModel : BindableBase
    {

        public SearchPageViewModel()
        {
            ViewModelManager.SearchPageViewModel = this;
            SearchProgressVisibility = Visibility.Visible;
            DataVisibility = Visibility.Hidden;
            NoResultVisibility = Visibility.Hidden;
            ClickBackCommand = new DelegateCommand(new Action(() => PageManager.SearchPage.NavigationService.GoBack()));
            ClickNeteaseCommand = new DelegateCommand<object>(new Action<object>(ClickNeteaseExecute));
            ClickKugouCommand = new DelegateCommand<object>(new Action<object>(ClickKugouExecute));
            ClickTencentCommand = new DelegateCommand<object>(new Action<object>(ClickTencentExecute));
            PrePlayCommand = new DelegateCommand<object>(new Action<object>(PrePlayExecute));

            //搜索选项加载
            IniConfig ini = new IniConfig("Config.ini");
            ini.Load();
            try
            {
                SearchOption = ini.GetObject<SearchOptionModel>();
            }
            catch (ArgumentException)
            {
                SearchOption = new SearchOptionModel { Kugou = true, Netease = true, Tencent = true };
                ini.SetObject(SearchOption);
                ini.Save();
            }
        }

        private string _searchContent;
        public string SearchContent
        {
            get { return _searchContent; }
            set
            {
                _searchContent = value;
                this.RaisePropertyChanged("SearchContent");
                SearchTask();
            }
        }

        private Visibility _searchProgressVisibility;
        public Visibility SearchProgressVisibility
        {
            get { return _searchProgressVisibility; }
            set
            {
                _searchProgressVisibility = value;
                this.RaisePropertyChanged("SearchProgressVisibility");
            }
        }


        private Visibility _dataVisibility;
        public Visibility DataVisibility
        {
            get { return _dataVisibility; }
            set
            {
                _dataVisibility = value;
                this.RaisePropertyChanged("DataVisibility");
            }
        }

        private Visibility _noResultVisibility;
        public Visibility NoResultVisibility
        {
            get { return _noResultVisibility; }
            set
            {
                _noResultVisibility = value;
                this.RaisePropertyChanged("NoResultVisibility");
            }
        }

        private ObservableCollection<IMusic> _musicInfos = new ObservableCollection<IMusic>();
        public ObservableCollection<IMusic> MusicInfos
        {
            get { return _musicInfos; }
            set
            {
                _musicInfos = value;
                this.RaisePropertyChanged("MusicInfos");
            }
        }

        private SearchOptionModel _searchOption;
        public SearchOptionModel SearchOption
        {
            get { return _searchOption; }
            set
            {
                _searchOption = value;
                this.RaisePropertyChanged("SearchOption");
            }
        }

        public DelegateCommand ClickBackCommand { get; set; }
        public DelegateCommand<object> ClickNeteaseCommand { get; set; }
        public DelegateCommand<object> ClickKugouCommand { get; set; }
        public DelegateCommand<object> ClickTencentCommand { get; set; }
        public DelegateCommand<object> PrePlayCommand { get; set; }

        public void PrePlayExecute(object parameter)
        {
            PlayerManager.Stop();
            if (!(parameter is IMusic) || !(parameter is IApi)) { return; }
            //PlayerManager.PlayMusic = parameter as IMusic;

            //数据库插入
            new MusicInfoManager().Insert(new MusicInfoModel(parameter as IMusic));

            //播放列表载入
            PlayerList.Add(parameter as IMusic);
            PlayerManager.PlayMusic = PlayerList.Current();

            //通知主窗口播放
            ViewModelManager.MainWindowViewModel.ClickPlayExecute();

        }

        public async void SearchTask()
        {
            //UI
            SearchProgressVisibility = Visibility.Visible;
            DataVisibility = Visibility.Hidden;
            NoResultVisibility = Visibility.Hidden;

            //清空当前绑定数据
            MusicInfos.Clear();
            IMusic[] infos;
            try
            {
                //网易云
                if (SearchOption.Netease)
                {
                    infos = await Task.Run(() => CoreApi.SearchMusic(MusicSource.Netease, SearchContent));
                    InsertMusicInfo(infos, 1);
                }

                //酷狗
                if (SearchOption.Kugou)
                {
                    infos = await Task.Run(() => CoreApi.SearchMusic(MusicSource.Kugou, SearchContent));
                    InsertMusicInfo(infos, 2);
                }

                //QQ音乐
                if (SearchOption.Tencent)
                {
                    infos = await Task.Run(() => CoreApi.SearchMusic(MusicSource.Tencent, SearchContent));
                    InsertMusicInfo(infos, 3);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"搜索失败！\n{e.Message}");
            }

            PlayerList.SetPreList(MusicInfos, "搜索");
            //UI
            SearchProgressVisibility = Visibility.Hidden;
            DataVisibility = MusicInfos.Count == 0 ? Visibility.Hidden : Visibility.Visible;
            NoResultVisibility = MusicInfos.Count == 0 ? Visibility.Visible : Visibility.Hidden;
        }

        public void InsertMusicInfo(IMusic[] insertInfos, int index)
        {
            for (int i = 0; i < insertInfos.Length; i++)
            {
                if (insertInfos[i].Id == string.Empty) { continue; }
                if (i * index + index - 1 < MusicInfos.Count) { MusicInfos.Insert(i * index + index - 1, insertInfos[i]); }
                else { MusicInfos.Add(insertInfos[i]); }
            }
        }

        public void ClickNeteaseExecute(object parameter)
        {
            SearchOption.Netease = (bool)parameter;
            var ini = new IniConfig("Config.ini");
            ini.SetObject(SearchOption);
            ini.Save();
            MusicInfos.Where(t => t.Origin == MusicSource.Netease).ToList().ForEach(item => MusicInfos.Remove(item));
            PlayerList.SetPreList(MusicInfos, "搜索");
        }

        public void ClickKugouExecute(object parameter)
        {
            SearchOption.Kugou = (bool)parameter;
            var ini = new IniConfig("Config.ini");
            ini.SetObject(SearchOption);
            ini.Save();
            MusicInfos.Where(t => t.Origin == MusicSource.Kugou).ToList().ForEach(item => MusicInfos.Remove(item));
            PlayerList.SetPreList(MusicInfos, "搜索");
        }

        public void ClickTencentExecute(object parameter)
        {
            SearchOption.Tencent = (bool)parameter;
            var ini = new IniConfig("Config.ini");
            ini.SetObject(SearchOption);
            ini.Save();
            MusicInfos.Where(t => t.Origin == MusicSource.Tencent).ToList().ForEach(item => MusicInfos.Remove(item));
            PlayerList.SetPreList(MusicInfos, "搜索");
        }
    }
}
