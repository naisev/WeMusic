using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeMusic.Enum;
using WeMusic.Interface;

namespace WeMusic.Model.MusicModel
{
    class NeteaseList : ISongList
    {
        public string name { get; set; }
        public string Title { get => name; set => name = value;  }
        public IMusic[] Musics { get ; set; }
        public string coverImgUrl { get; set; }
        public string CoverUrl { get => coverImgUrl;  set => coverImgUrl = value;  }
        public string id { get; set; }
        public string Id { get => id; set => id = value; }
        public MusicSource Origin { get => MusicSource.Netease;  }
        public string SourceName { get => "网易云";  }
    }
}
