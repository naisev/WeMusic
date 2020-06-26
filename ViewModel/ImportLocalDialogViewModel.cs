using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeMusic.Control;
using WeMusic.Model;
using WeMusic.Model.DbModel;
using WeMusic.Model.MusicModel;

namespace WeMusic.ViewModel
{
    class ImportLocalDialogViewModel : BindableBase
    {
        public ImportLocalDialogViewModel()
        {
            ClickChooseMusicCommand = new DelegateCommand(new Action(ClickChooseMusicExecute));
            ClickScanFolderCommand = new DelegateCommand(new Action(ClickScanFolderExecute));
        }
        public DelegateCommand ClickChooseMusicCommand { get; set; }
        public DelegateCommand ClickScanFolderCommand { get; set; }

        public void ClickChooseMusicExecute()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Title = "请选择音乐文件";
            dialog.Filter = "mp3文件(*.mp3)|*.mp3|wav文件(*.wav)|*.wav";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string path = dialog.FileName;
                TagLib.Tag tag = TagLib.File.Create(path).Tag;

                //获取艺术家
                string artists = string.Empty;
                foreach (var item in tag.AlbumArtists)
                {
                    artists += "、" + item;
                }
                if (artists != string.Empty) { artists=artists.Substring(1); }

                string id = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(File.ReadAllBytes(path))).Replace("-","");
                
                bool result=new MusicInfoManager().Insert(new MusicInfoModel
                {
                    Album = tag.Album,
                    Artists =artists,
                    Name = tag.Title==string.Empty?Path.GetFileNameWithoutExtension(path):tag.Title,
                    CoverId = string.Empty,
                    Origin = Enum.MusicSource.Local,
                    SourceName = "本地音乐",
                    Id = id
                });

                //检查id是否重复
                if (result)
                {
                    new LocalListManager().Insert(new LocalListModel
                    {
                        FilePath = path,
                        Id = id
                    });
                    Toast.Show("添加本地音乐成功！", Toast.InfoType.Success);
                    ViewModelManager.BasePageViewModel.ClickLocalMusicExecute();
                }
                else
                {
                    Toast.Show("本地音乐已存在相同音乐！", Toast.InfoType.Error);
                }
                
            }
        }

        public void ClickScanFolderExecute()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择扫描音乐文件夹";
            if (dialog.ShowDialog() != DialogResult.OK||dialog.SelectedPath==string.Empty)
            {
                return;
            }
            var files = Directory.GetFiles(dialog.SelectedPath);
            int amount = 0;
            foreach (var item in files)
            {
                if(Path.GetExtension(item)!=".mp3"&& Path.GetExtension(item) != ".wav") { continue; }
                TagLib.Tag tag = TagLib.File.Create(item).Tag;

                //获取艺术家
                string artists = string.Empty;
                foreach (var ar in tag.AlbumArtists)
                {
                    artists += "、" + ar;
                }
                if (artists != string.Empty) { artists = artists.Substring(1); }

                string id = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(File.ReadAllBytes(item))).Replace("-", "");

                bool result = new MusicInfoManager().Insert(new MusicInfoModel
                {
                    Album = tag.Album==null?"":tag.Album,
                    Artists = artists,
                    Name = string.IsNullOrEmpty(tag.Title) ? Path.GetFileNameWithoutExtension(item) : tag.Title,
                    CoverId = string.Empty,
                    Origin = Enum.MusicSource.Local,
                    SourceName = "本地音乐",
                    Id = id
                });

                //检查id是否重复
                if (result)
                {
                    new LocalListManager().Insert(new LocalListModel
                    {
                        FilePath = item,
                        Id = id
                    });
                    amount++;
                }
            }

            Toast.Show($"共添加本地音乐{amount}首！", Toast.InfoType.Success);
            ViewModelManager.BasePageViewModel.ClickLocalMusicExecute();
        }
    }
}
