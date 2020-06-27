using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Prism.Commands;
using Prism.Mvvm;
using WeMusic.Control;
using WeMusic.Interface;
using WeMusic.Model;
using WeMusic.Model.DbModel;
using WeMusic.Model.MusicModel;
using WeMusic.Model.Player;

namespace WeMusic.ViewModel
{
    public class BasePageViewModel : BindableBase
    {
        private class TagInfo
        {
            /// <summary>
            /// 0：自定义歌单，1：平台歌单
            /// </summary>
            public int Kind { get; set; }
            public string Value { get; set; }
            public TagInfo(int kind,string value)
            {
                Kind = kind;
                Value = value;
            }
        }
        public BasePageViewModel()
        {
            ViewModelManager.BasePageViewModel = this;
            DefaultListCommand = new DelegateCommand(new Action(DefaultListExecute));
            PrePlayCommand = new DelegateCommand<object>(new Action<object>(PrePlayExecute));
            AddListCommand = new DelegateCommand(new Action(AddListExecute));
            ClickImportListCommand = new DelegateCommand(new Action(ClickImportListExecute));
            ClickLocalMusicCommand = new DelegateCommand(new Action(ClickLocalMusicExecute));
            ClickPlayAllCommand = new DelegateCommand(new Action(ClickPlayAllExecute));
            ClickDownloadAllCommand = new DelegateCommand(new Action(ClickDownloadAllExecute));
            OpenPopupCommand = new DelegateCommand(new Action(OpenPopupExecute));
            ClickImportLocalCommand = new DelegateCommand(new Action(ClickImportLocalExecute));
            DefaultListExecute();
            RefreshCustomList();
            RefreshPlatformList();
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

        private StackPanel _customList = new StackPanel();
        public StackPanel CustomList
        {
            get { return _customList; }
            set
            {
                _customList = value;
                this.RaisePropertyChanged("CustomList");
            }
        }

        private StackPanel _platformList;
        public StackPanel PlatformList
        {
            get { return _platformList; }
            set
            {
                _platformList = value;
                this.RaisePropertyChanged("PlatformList");
            }
        }

        public string ListId { get; set; }

        public DelegateCommand DefaultListCommand { get; set; }
        public DelegateCommand<object> PrePlayCommand { get; set; }
        public DelegateCommand AddListCommand { get; set; }
        public DelegateCommand ClickImportListCommand { get; set; }
        public DelegateCommand ClickLocalMusicCommand { get; set; }
        public DelegateCommand ClickPlayAllCommand { get; set; }
        public DelegateCommand ClickDownloadAllCommand { get; set; }
        public DelegateCommand ClickAddToCommand { get; set; }
        public DelegateCommand OpenPopupCommand { get; set; }
        public DelegateCommand ClickImportLocalCommand { get; set; }

        public void DefaultListExecute()
        {
            ListId = string.Empty;
            var orm = new DefaultListManager();
            var list = orm.GetList();
            MusicInfos.Clear();
            list.ForEach(item =>
            {
                var mif = new MusicInfoManager();
                var music = mif.Find(item.Id);
                MusicInfos.Add(music.ToIMusic());
            });
            PlayerList.SetPreList(MusicInfos, "默认列表");
            DataGridAnimation();
        }
        public void PrePlayExecute(object parameter)
        {
            PlayerManager.Stop();
            if (!(parameter is IMusic) || !(parameter is IApi)) { return; }
            //PlayerManager.PlayMusic = parameter as IMusic;

            //数据库插入
            new MusicInfoManager().Insert(new MusicInfoModel(parameter as IMusic));

            //播放列表载入
            PlayerList.SetList();
            PlayerList.SetCurrentMusic(parameter as IMusic);
            PlayerManager.PlayMusic = PlayerList.Current();

            //通知主窗口播放
            PlayerManager.Play();
        }
        public async void AddListExecute()
        {
            string name = (string)await DialogManager.ShowCreateListDialog();
            if (name == null || name == string.Empty) { return; }
            bool result = new CustomTitleManager().Insert(new CustomTitleModel(name));
            if (result)
            {
                RefreshCustomList();
                Toast.Show($"添加歌单{name}成功！", Toast.InfoType.Success);
            }
            else
            {
                Toast.Show("添加失败！歌单名重复！", Toast.InfoType.Error);
            }
        }

        public void RefreshCustomList()
        {
            CustomList = new StackPanel();
            var titles = new CustomTitleManager().GetList();
            titles.ForEach(item =>
            {
                ImageRadioButton btn = new ImageRadioButton();
                btn.SetValue(ImageRadioButton.StyleProperty, Application.Current.Resources["MenuRadioButtom"]);
                btn.GroupName = "MenuItem";
                btn.Content = item.Title;
                var menu = new ContextMenu();
                var title = new TextBlock { Text = "删除歌单" };
                title.MouseLeftButtonUp += ClickDeleteList;
                title.Tag = new TagInfo(0, item.Title);
                menu.Items.Add(title);
                btn.ContextMenu = menu;
                btn.Command = new DelegateCommand<object>(new Action<object>(ClickCustomList));
                btn.CommandParameter = item.Title;
                CustomList.Children.Add(btn);
            });
        }

        private void ClickDeleteList(object sender, MouseButtonEventArgs e)
        {
            TagInfo tag = (sender as TextBlock).Tag as TagInfo;
            if (tag.Kind == 0)
            {
                new CustomListManager().CurrentDb.Delete(o => o.Title == tag.Value);
                new CustomTitleManager().Delete(tag.Value);
                RefreshCustomList();
                if (PlayerList.PreListTitle == tag.Value)
                {
                    DefaultListExecute();
                }
            }
            else
            {
                new PlatformInfoManager().Delete(tag.Value);
                new PlatformListManager().CurrentDb.Delete(o => o.PlatformId == tag.Value);
                RefreshPlatformList();
                if (ListId == tag.Value)
                {
                    DefaultListExecute();
                }
            }
            Toast.Show("删除歌单成功！", Toast.InfoType.Success);
        }

        public void RefreshPlatformList()
        {
            PlatformList = new StackPanel();
            var infos = new PlatformInfoManager().GetList();
            infos.ForEach(item =>
            {
                ImageRadioButton btn = new ImageRadioButton();
                btn.SetValue(ImageRadioButton.StyleProperty, Application.Current.Resources["MenuRadioButtom"]);
                btn.GroupName = "MenuItem";
                btn.Content = item.Title;
                var menu = new ContextMenu();
                var title = new TextBlock { Text = "删除歌单" };
                title.MouseLeftButtonUp += ClickDeleteList;
                title.Tag = new TagInfo(1, item.Id);
                menu.Items.Add(title);
                btn.ContextMenu = menu;
                btn.Command = new DelegateCommand<object>(new Action<object>(ClickPlatformList));
                btn.CommandParameter = item.Id;
                PlatformList.Children.Add(btn);
            });
        }

        public void ClickCustomList(object parameter)
        {
            ListId = string.Empty;
            string title = parameter.ToString();
            var ls = new CustomListManager().GetList();
            MusicInfos.Clear();
            ls.ForEach(item =>
            {
                if (item.Title == title)
                {
                    var mim = new MusicInfoManager();
                    var music = mim.Find(item.Id);
                    MusicInfos.Add(music.ToIMusic());
                }
            });
            PlayerList.SetPreList(MusicInfos, title);
            DataGridAnimation();
        }

        public void ClickPlatformList(object parameter)
        {
            string id = parameter.ToString();
            ListId = id;
            var ls = new PlatformListManager().GetList();
            MusicInfos.Clear();
            ls.ForEach(item =>
            {
                if (item.PlatformId == id)
                {
                    var mim = new MusicInfoManager();
                    var music = mim.Find(item.MusicId);
                    MusicInfos.Add(music.ToIMusic());
                }
            });
            PlayerList.SetPreList(MusicInfos, new PlatformInfoManager().Find(id).Title);
            DataGridAnimation();
        }

        public void DataGridAnimation()
        {
            ThicknessAnimation upper = new ThicknessAnimation(new Thickness(0, 100, 0, 0), new Thickness(0), new TimeSpan(0, 0, 0, 0, 300));
            upper.DecelerationRatio = 1;
            PageManager.BasePage?.dgv?.BeginAnimation(DataGrid.MarginProperty, upper);
        }

        public void RefreshShowList(string title)
        {
            //如果当前BasePage的DataGrid展示的是默认列表，进行刷新
            if (PlayerList.PreListTitle != title)
            {
                return;
            }
            if (ListId == string.Empty)
            {
                if (title == "默认列表")
                {
                    var orm = new DefaultListManager();
                    var list = orm.GetList();
                    MusicInfos.Clear();
                    list.ForEach(item =>
                    {
                        var mif = new MusicInfoManager();
                        var music = mif.Find(item.Id);
                        MusicInfos.Add(music.ToIMusic());
                    });
                }
                else if (title == "本地音乐")
                {
                    MusicInfos.Clear();
                    var list = new LocalListManager().GetList();
                    list.ForEach(item =>
                    {
                        var mim = new MusicInfoManager();
                        var music = mim.Find(item.Id);
                        MusicInfos.Add(music.ToIMusic());
                    });
                }
                else
                {
                    var ls = new CustomListManager().GetList();
                    MusicInfos.Clear();
                    ls.ForEach(item =>
                    {
                        if (item.Title == title)
                        {
                            var mim = new MusicInfoManager();
                            var music = mim.Find(item.Id);
                            MusicInfos.Add(music.ToIMusic());
                        }
                    });
                }
            }
            else
            {
                var ls = new PlatformListManager().GetList();
                MusicInfos.Clear();
                ls.ForEach(item =>
                {
                    if (item.PlatformId == ListId)
                    {
                        var mim = new MusicInfoManager();
                        var music = mim.Find(item.MusicId);
                        MusicInfos.Add(music.ToIMusic());
                    }
                });
            }
            
        }

        public async void ClickImportListExecute()
        {
            await DialogManager.ShowPlatformListDialog();
        }

        public void ClickLocalMusicExecute()
        {
            ListId = string.Empty;
            MusicInfos.Clear();
            var list = new LocalListManager().GetList();
            list.ForEach(item =>
            {
                var mim = new MusicInfoManager();
                var music = mim.Find(item.Id);
                MusicInfos.Add(music.ToIMusic());
            });
            PlayerList.SetPreList(MusicInfos, "本地音乐");
            DataGridAnimation();
        }

        public void ClickPlayAllExecute()
        {
            if(MusicInfos is null || MusicInfos.Count <= 0) { return; }
            PrePlayExecute(MusicInfos[0]);
        }

        public async void ClickDownloadAllExecute()
        {
            await DialogManager.ShowDownloadDialog(MusicInfos);
        }

        public void AddToCustomList(object parameter)
        {
            if (parameter.ToString() == PlayerList.PreListTitle) { Toast.Show("添加失败！歌单相同！", Toast.InfoType.Error); return; }

            if (MusicInfos is null) { return; }
            //将音乐加入到自定义列表数据库
            var orm = new CustomListManager();
            var mim = new MusicInfoManager();
            foreach (var item in MusicInfos)
            {
                orm.Insert(new CustomListModel(parameter.ToString(), item.Id));
                mim.Insert(new MusicInfoModel(item));
            }
            ViewModelManager.BasePageViewModel.RefreshShowList(parameter.ToString());
            Toast.Show("添加成功！", Toast.InfoType.Success);
        }

        public void AddToDefaultList()
        {
            if ("默认列表" == PlayerList.PreListTitle) { Toast.Show("添加失败！歌单相同！", Toast.InfoType.Error); return; }

            if (MusicInfos is null) { return; }
            //在默认列表数据库中加入音乐
            var dlm = new DefaultListManager();
            var mim = new MusicInfoManager();
            foreach (var item in MusicInfos)
            {
                dlm.Insert(new DefaultListModel( item.Id));
                mim.Insert(new MusicInfoModel(item));
            }

            //如果当前BasePage的DataGrid展示的是默认列表，进行刷新
            ViewModelManager.BasePageViewModel.RefreshShowList("默认列表");
            Toast.Show("添加成功！", Toast.InfoType.Success);
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

        public async void ClickImportLocalExecute()
        {
            await DialogManager.ShowImportLocalDialog();
        }
    }
}
