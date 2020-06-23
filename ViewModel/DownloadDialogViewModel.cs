
using Masuit.Tools.Net;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeMusic.Control;
using WeMusic.Interface;
using WeMusic.Model;

namespace WeMusic.ViewModel
{
    class DownloadDialogViewModel : BindableBase
    {
        public DownloadDialogViewModel()
        {
            ChangeDirectoryCommand = new DelegateCommand(new Action(ChangeDirectoryExecute));
            ClickDownloadCommand = new DelegateCommand(new Action(ClickDownloadExecute));
        }
        private ObservableCollection<IMusic> _musics = new ObservableCollection<IMusic>();
        public ObservableCollection<IMusic> Musics
        {
            get { return _musics; }
            set
            {
                _musics = value;
                this.RaisePropertyChanged("Musics");
            }
        }

        private string _saveDirectory = DownloadManager.MusicDownloadPath;
        public string SaveDirectory
        {
            get { return _saveDirectory; }
            set
            {
                _saveDirectory = value;
                this.RaisePropertyChanged("SaveDirectory");
            }
        }

        public DelegateCommand ChangeDirectoryCommand { get; set; }
        public DelegateCommand ClickDownloadCommand { get; set; }

        public void ChangeDirectoryExecute()
        {
            System.Windows.Forms.FolderBrowserDialog openFileDialog = new System.Windows.Forms.FolderBrowserDialog();  //选择文件夹
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)//注意，此处一定要手动引入System.Window.Forms空间，否则你如果使用默认的DialogResult会发现没有OK属性
            {
                SaveDirectory = openFileDialog.SelectedPath;
                DownloadManager.MusicDownloadPath = openFileDialog.SelectedPath;
            }
        }

        public void ClickDownloadExecute()
        {
            //检查目录是否存在
            if (!Directory.Exists(SaveDirectory))
            {
                Toast.Show("目录不存在！请重新选择！", Toast.InfoType.Error);
                return;
            }
            if (SaveDirectory != DownloadManager.MusicDownloadPath)
            {
                DownloadManager.MusicDownloadPath = SaveDirectory;
            }
            foreach (var item in Musics)
            {
                if (File.Exists(DownloadManager.MusicCachePath + item.Id + ".tmp"))
                {
                    try
                    {
                        File.Copy(DownloadManager.MusicCachePath + item.Id + ".tmp", $"{DownloadManager.MusicDownloadPath}{item.Name} - {item.Artists}.mp3");
                        Toast.Show($"{item.Name} - {item.Artists}.mp3 下载成功",Toast.InfoType.Success);
                    }
                    catch
                    {
                        Toast.Show($"{item.Name} - {item.Artists}.mp3 下载失败", Toast.InfoType.Error);
                    }
                    
                }
                else
                {
                    DownloadManager.DownloadFileAsync((item as IApi).GetMusicUrl(), DownloadManager.MusicDownloadPath, $"{item.Name} - {item.Artists}.mp3", null,
                        new Action<object, int>((o, i) => Toast.Show($"{item.Name} - {item.Artists}.mp3 下载成功", Toast.InfoType.Success)));
                }
            }
        }
    }
}
