using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WeMusic.Control;
using WeMusic.Enum;
using WeMusic.Interface;
using WeMusic.Model;
using WeMusic.Model.DbModel;

namespace WeMusic.ViewModel
{
    class PlatformListDialogViewModel : BindableBase
    {
        public const string EXP_SINGLE_KUGOU = "说明：打开酷狗音乐->选择歌单->分享->复制链接(电脑端选择微博->复制其中的链接)";
        public const string EXP_SINGLE_NETEASE = "说明：打开网易云音乐->选择歌单->分享->复制链接";
        public const string EXP_SINGLE_TENCENT = "说明：打开QQ音乐->选择歌单->分享->复制链接";
        public const string EXP_PLATFORM_KUGOU = "说明：打开https://www.kugou.com/->登录->个人账号->kgopen后的数字就是ID";
        public const string EXP_PLATFORM_NETEASE = "说明：打开http://music.163.com/->登录->个人中心->浏览器地址栏id=后面的数字就是ID";
        public const string EXP_PLATFORM_TENCENT = "说明：您的QQ号就是ID";
        public PlatformListDialogViewModel()
        {
            ClickImportCommand = new DelegateCommand(new Action(ClickImportExecute));
        }
        private bool _chooseKugou;
        public bool ChooseKugou
        {
            get { return _chooseKugou; }
            set
            {
                _chooseKugou = value;
                this.RaisePropertyChanged("ChooseKugou");
                if (!value) { return; }
                if (ChooseSingle) { Explanation = EXP_SINGLE_KUGOU; }
                else { Explanation = EXP_PLATFORM_KUGOU; }
            }
        }

        private bool _chooseNetease = true;
        public bool ChooseNetease
        {
            get { return _chooseNetease; }
            set
            {
                _chooseNetease = value;
                this.RaisePropertyChanged("ChooseNetease");
                if (!value) { return; }
                if (ChooseSingle) { Explanation = EXP_SINGLE_NETEASE; }
                else { Explanation = EXP_PLATFORM_NETEASE; }
            }
        }

        private bool _chooseTencent;
        public bool ChooseTencent
        {
            get { return _chooseTencent; }
            set
            {
                _chooseTencent = value;
                this.RaisePropertyChanged("ChooseTencent");
                if (!value) { return; }
                if (ChooseSingle) { Explanation = EXP_SINGLE_TENCENT; }
                else { Explanation = EXP_PLATFORM_TENCENT; }
            }
        }

        private bool _chooseSingle = true;
        public bool ChooseSingle
        {
            get { return _chooseSingle; }
            set
            {
                _chooseSingle = value;
                this.RaisePropertyChanged("ChooseSingle");
            }
        }

        private bool _choosePlatform;
        public bool ChoosePlatform
        {
            get { return _choosePlatform; }
            set
            {
                _choosePlatform = value;
                this.RaisePropertyChanged("ChoosePlatform");
            }
        }

        private string _importUrl;
        public string ImportUrl
        {
            get { return _importUrl; }
            set
            {
                _importUrl = value;
                this.RaisePropertyChanged("ImportUrl");
            }
        }

        private string _explanation = EXP_SINGLE_NETEASE;
        public string Explanation
        {
            get { return _explanation; }
            set
            {
                _explanation = value;
                this.RaisePropertyChanged("Explanation");
            }
        }

        private Visibility _importButtonVisibility = Visibility.Visible;
        public Visibility ImportButtonVisibility
        {
            get { return _importButtonVisibility; }
            set
            {
                _importButtonVisibility = value;
                this.RaisePropertyChanged("ImportButtonVisibility");
            }
        }

        private Visibility _loadingButtonVisibility = Visibility.Hidden;
        public Visibility LoadingButtonVisibility
        {
            get { return _loadingButtonVisibility; }
            set
            {
                _loadingButtonVisibility = value;
                this.RaisePropertyChanged("LoadingButtonVisibility");
            }
        }


        public DelegateCommand ClickImportCommand { get; set; }
        
        public async void ClickImportExecute()
        {
            
            //获取类型
            MusicSource source;
            if (ChooseKugou) { source = MusicSource.Kugou; }
            else if (ChooseNetease) { source = MusicSource.Netease; }
            else { source = MusicSource.Tencent; }

            ISongList songList = null;
            bool isError = false;
            await Task.Run(() =>
            {
                Thread.Sleep(300);
                ImportButtonVisibility = Visibility.Hidden;
                LoadingButtonVisibility = Visibility.Visible;
                try
                {
                    songList = CoreApi.GetPlatformSongList(source, ImportUrl);
                }
                catch
                {
                    isError = true;
                }
                ImportButtonVisibility = Visibility.Visible;
                LoadingButtonVisibility = Visibility.Hidden;
            });
            if (isError)
            {
                Toast.Show("导入失败！", Toast.InfoType.Error);
            }
            else
            {
                new PlatformInfoManager().Insert(new PlatformInfoModel(songList));
                Toast.Show("导入成功!", Toast.InfoType.Success);
            }
            
        }

        
    }
}
