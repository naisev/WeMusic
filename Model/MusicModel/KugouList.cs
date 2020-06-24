using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeMusic.Enum;
using WeMusic.Interface;

namespace WeMusic.Model.MusicModel
{
    class KugouList : ISongList
    {
        public string specialname { get; set; }
        public string Title { get => specialname; set => specialname = value; }
        public IMusic[] Musics { get; set; }
        public string imgurl { get; set; }
        public string CoverUrl { get => imgurl; set => imgurl=value; }
        public string id { get; set; }
        public string Id { get => id; set => id = value; }
        public MusicSource Origin { get { return MusicSource.Kugou; } }
        public string SourceName { get { return "酷狗音乐"; } }
        public KugouList(ISongList songlist)
        {
            Title = songlist.Title;
            Musics = songlist.Musics;
            CoverUrl = songlist.CoverUrl;
            Id = songlist.Id;
        }
        public KugouList()
        {

        }
    }
}
