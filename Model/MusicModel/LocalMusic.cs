using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeMusic.Enum;
using WeMusic.Interface;
using WeMusic.Model.DbModel;

namespace WeMusic.Model.MusicModel
{
    class LocalMusic : IMusic, IApi
    {
        private string path { get; set; }
        public string Id { get ; set ; }
        public string CoverId { get ; set; }
        public string Artists { get ; set ; }
        public string Name { get; set ; }
        public string Album { get ; set ; }

        public string SourceName { get => "本地音乐"; }

        public MusicSource Origin { get => MusicSource.Local; }

        public string GetCoverUrl()
        {
            return string.Empty;
        }

        public string GetLyric()
        {
            return "本地音乐暂无歌词";
        }

        public string GetMusicUrl()
        {
            return path;
        }

        public LocalMusic(IMusic music)
        {
            Id = music.Id;
            Name = music.Name;
            Album = music.Album;
            Artists = music.Artists;
            CoverId = Id;
            path = new LocalListManager().Find(Id).FilePath;
        }

        public LocalMusic(string path) { this.path = path; }
    }
}
