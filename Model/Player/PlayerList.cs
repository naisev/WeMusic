using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeMusic.Enum;
using WeMusic.Interface;
using WeMusic.ViewModel;

namespace WeMusic.Model.Player
{
    class PlayerList
    {
        public static List<IMusic> Musics { get; private set; } = new List<IMusic>();
        private static List<IMusic> preMusics = new List<IMusic>();
        private static int index = 0;
        public static string PreListTitle { get; set; }
        public static string ListTitle { get; set; }

        public static PlayMode Mode { get; set; } = PlayMode.ListLoop;

        /// <summary>
        /// 设置播放列表
        /// </summary>
        /// <param name="musics"></param>
        public static void SetList(IEnumerable<IMusic> musics,string title)
        {
            Musics = new List<IMusic>(musics);
            ListTitle = title;
            ViewModelManager.MainWindowViewModel.ShowMusicList = new ObservableCollection<IMusic>(musics);
            ViewModelManager.MainWindowViewModel.MusicListTitle = ListTitle;
        }

        /// <summary>
        /// 从预播放列表 设置播放列表
        /// </summary>
        public static void SetList()
        {
            Musics = new List<IMusic>(preMusics);
            ListTitle = PreListTitle;
            ViewModelManager.MainWindowViewModel.ShowMusicList = new ObservableCollection<IMusic>(Musics);
            ViewModelManager.MainWindowViewModel.MusicListTitle = ListTitle;
        }

        /// 设置预播放列表
        /// </summary>
        /// <param name="musics"></param>
        public static void SetPreList(IEnumerable<IMusic> musics,string title)
        {
            preMusics = new List<IMusic>(musics);
            PreListTitle = title;
        }

        /// <summary>
        /// 获得下一首歌曲
        /// 如果当前序号是最后一首歌，会抛出异常
        /// </summary>
        /// <returns></returns>
        public static IMusic Next()
        {
            switch (Mode)
            {
                case PlayMode.SinglePlay:
                    throw new Exception();
                case PlayMode.ListLoop:
                    if (index + 1 >= Musics.Count)
                    {
                        index = 0;
                    }
                    else
                    {
                        index++;
                    }
                    break;
                case PlayMode.RandomLoop:
                    index = new Random().Next(0, Musics.Count);
                    break;
            }
            try
            {
                return Musics[index];
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 获得上一首歌曲
        /// 如果当前序号是第一首歌，会抛出异常
        /// </summary>
        /// <returns></returns>
        public static IMusic Previous()
        {
            switch (Mode)
            {
                case PlayMode.SinglePlay:
                    throw new Exception();
                case PlayMode.ListLoop:
                    if (index < 1)
                    {
                        index = Musics.Count - 1;
                    }
                    else
                    {
                        index--;
                    }
                    break;
                case PlayMode.RandomLoop:
                    index = new Random().Next(0, Musics.Count);
                    break;
            }
            try
            {
                return Musics[index];
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 获得当前歌曲
        /// 如果列表为空，会抛出异常
        /// </summary>
        /// <returns></returns>
        public static IMusic Current()
        {
            if (index < 0 || index >= Musics.Count) { throw new Exception(); }
            return Musics[index];
        }

        /// <summary>
        /// 获取当前播放序号，从1开始
        /// </summary>
        /// <returns></returns>
        public static int GetIndex()
        {
            return index + 1;
        }

        /// <summary>
        /// 在当前播放歌曲后插入
        /// </summary>
        /// <param name="music"></param>
        public static void Insert(IMusic music, bool play = true)
        {
            Musics.Insert(index, music);
            if (play) { index++; }
        }

        /// <summary>
        /// 在当前播放列表加入
        /// </summary>
        /// <param name="music"></param>
        public static void Add(IMusic music, bool play = true)
        {
            Musics.Add(music);
            if (play) { index = Musics.Count - 1; }
        }

        /// <summary>
        /// 设置当前播放音乐
        /// </summary>
        /// <param name="music"></param>
        public static void SetCurrentMusic(IMusic music)
        {
            for (int i = 0; i < Musics.Count; i++)
            {
                if (Musics[i] == music)
                {
                    index = i;
                }
            }
        }
    }
}
