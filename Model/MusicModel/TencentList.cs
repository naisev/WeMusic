using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeMusic.Enum;
using WeMusic.Interface;

namespace WeMusic.Model.MusicModel
{
    class TencentList : ISongList
    {
        public string dissname { get; set; }
        public string Title { get => dissname; set => dissname = value; }
        public IMusic[] Musics { get; set; }
        public string logo { get; set; }
        public string CoverUrl { get => logo; set => logo = value; }
        public string disstid { get; set; }
        public string Id { get => disstid; set => disstid = value; }
        public MusicSource Origin { get { return MusicSource.Tencent; } }
        public string SourceName { get { return "QQ音乐"; } }
    }
}
