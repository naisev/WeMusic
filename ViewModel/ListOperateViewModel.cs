using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WeMusic.Control;
using WeMusic.Interface;
using WeMusic.Model;
using WeMusic.Model.DbModel;
using WeMusic.Model.Player;

namespace WeMusic.ViewModel
{
    class ListOperateViewModel : BindableBase
    {
        public ListOperateViewModel()
        {
            PrePlayCommand = new DelegateCommand<object>(new Action<object>(PrePlayExecute));
            PreMenuCommand = new DelegateCommand<object>(new Action<object>(PreMenuExecute));
            DownloadCommand = new DelegateCommand<object>(new Action<object>(DownloadExecute));
            OpenPopupCommand = new DelegateCommand(new Action(OpenPopupExecute));
            DeleteCommand = new DelegateCommand<object>(new Action<object>(DeleteExecute));
        }

        private ObservableCollection<object> _menus;

        public ObservableCollection<object> Menus
        {
            get { return _menus; }
            set
            {
                _menus = value;
                this.RaisePropertyChanged("Menus");
            }
        }

        private object menuParameter = null;

        public DelegateCommand<object> PrePlayCommand { get; set; }
        public DelegateCommand<object> PreMenuCommand { get; set; }
        public DelegateCommand<object> DownloadCommand { get; set; }
        public DelegateCommand OpenPopupCommand { get; set; }
        public DelegateCommand<object> DeleteCommand { get; set; }

        public void PrePlayExecute(object parameter)
        {
            PlayerManager.Stop();
            if (!(parameter is IMusic) || !(parameter is IApi)) { return; }

            //数据库插入
            new MusicInfoManager().Insert(new MusicInfoModel(parameter as IMusic));

            //播放列表载入
            PlayerList.SetList();
            PlayerList.SetCurrentMusic(parameter as IMusic);
            PlayerManager.PlayMusic = PlayerList.Current();

            //通知主窗口播放
            PlayerManager.Play();
        }

        public void AddToDefaultList()
        {
            if ("默认列表" == PlayerList.PreListTitle) { Toast.Show("添加失败！歌单相同！", Toast.InfoType.Error); return; }
            if (menuParameter is null) { return; }
            //在默认列表数据库中加入音乐
            var dlm = new DefaultListManager();
            dlm.Insert(new DefaultListModel((menuParameter as IMusic).Id));
            var mim = new MusicInfoManager();
            mim.Insert(new MusicInfoModel(menuParameter as IMusic));

            //如果当前BasePage的DataGrid展示的是默认列表，进行刷新
            ViewModelManager.BasePageViewModel.RefreshShowList("默认列表");
            Toast.Show("添加成功！", Toast.InfoType.Success);
        }

        public void PreMenuExecute(object parameter)
        {
            menuParameter = parameter;
        }

        public void AddToCustomList(object parameter)
        {
            if (parameter.ToString() == PlayerList.PreListTitle) { Toast.Show("添加失败！歌单相同！", Toast.InfoType.Error); return; }
            if (menuParameter is null) { return; }
            //将音乐加入到自定义列表数据库
            var orm = new CustomListManager();
            orm.Insert(new CustomListModel(parameter.ToString(), (menuParameter as IMusic).Id));
            var mim = new MusicInfoManager();
            mim.Insert(new MusicInfoModel(menuParameter as IMusic));
            ViewModelManager.BasePageViewModel.RefreshShowList(parameter.ToString());
            Toast.Show("添加成功！", Toast.InfoType.Success);
        }

        public async void DownloadExecute(object parameter)
        {
            await DialogManager.ShowDownloadDialog(parameter);
        }

        public void OpenPopupExecute()
        {
            Menus = new ObservableCollection<object>();

            Menus.Add(new MenuItem
            {
                Header = "默认列表",
                Command = new DelegateCommand(new Action(AddToDefaultList))
            });
            Menus.Add(new Separator());
            var titles = new CustomTitleManager().GetList();
            titles.ForEach(item =>
            {
                Menus.Add(new MenuItem
                {
                    Header = item.Title,
                    Command = new DelegateCommand<object>(new Action<object>(AddToCustomList)),
                    CommandParameter = item.Title
                });
            });
        }

        public void DeleteExecute(object parameter)
        {
            //获取音乐id
            if (parameter is null) { return; }
            if (!(menuParameter is IMusic)) { return; }
            var music = menuParameter as IMusic;

            //检查是否搜索页面
            if (PageManager.CurrentPage == PageManager.SearchPage)
            {
                ViewModelManager.SearchPageViewModel.MusicInfos.Remove(music);
            }
            else
            {
                //检查是否平台音乐
                if (ViewModelManager.BasePageViewModel.ListId == string.Empty)
                {
                    switch (PlayerList.PreListTitle)
                    {
                        case "默认列表":
                            new DefaultListManager().Delete(music.Id);
                            ViewModelManager.BasePageViewModel.RefreshShowList("默认列表");
                            break;
                        case "本地音乐":
                            new LocalListManager().Delete(music.Id);
                            ViewModelManager.BasePageViewModel.RefreshShowList("本地音乐");
                            break;
                        default:
                            new CustomListManager().CurrentDb.Delete((o) => o.Title == PlayerList.PreListTitle && o.Id == music.Id);
                            ViewModelManager.BasePageViewModel.RefreshShowList(PlayerList.PreListTitle);
                            break;
                    }
                }
                else
                {
                    new PlatformListManager().CurrentDb.Delete((o) => o.PlatformId == ViewModelManager.BasePageViewModel.ListId && o.MusicId == music.Id);
                    ViewModelManager.BasePageViewModel.RefreshShowList(PlayerList.PreListTitle);
                }
            }

            Toast.Show("移除成功！", Toast.InfoType.Success);
        }
    }
}
