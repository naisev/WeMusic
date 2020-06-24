using Native.Tool.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeMusic.Enum;
using WeMusic.Interface;

namespace WeMusic.Model.MusicModel
{
    public class TencentMusic : IMusic,IApi
    {
        public string songmid { get; set; }
        public string Id
        {
            get { return songmid; }
            set { songmid = value; }
        }
        public string songname { get; set; }
        public string Name
        {
            get { return songname; }
            set { songname = value; }
        }
        public TencentSinger[] singer { get; set; }
        public string Artists
        {
            get
            {
                string value = string.Empty;
                foreach (var tmp in singer) value += "、" + tmp.name;
                if (value != string.Empty) { value = value.Substring(1); }
                return value;
            }
            set
            {
                string[] tmps = value.Split('、');
                singer = new TencentSinger[tmps.Length];
                for (int i = 0; i < singer.Length; i++)
                {
                    singer[i] = new TencentSinger { name = tmps[i] };
                }
            }
        }
        public string albumname { get; set; }
        public string Album
        {
            get { return albumname; }
            set { albumname = value; }
        }
        public string albummid { get; set; }
        public string source { get; set; }
        public string SourceName { get { return "QQ音乐"; } }
        public MusicSource Origin { get { return MusicSource.Tencent; } }
        public string CoverId { get => albummid; set => albummid = value; }

        public string GetCoverUrl()
        {
            return $"https://y.gtimg.cn/music/photo_new/T002R300x300M000{CoverId}.jpg";
        }

        public string GetLyric()
        {
            string url = $"https://api.qq.jsososo.com/lyric?songmid={Id}";
            byte[] data = HttpWebClient.Get(url);
            JObject json = JObject.Parse(Encoding.UTF8.GetString(data));
            return json["data"]["lyric"].ToString();
        }

        public string GetMusicUrl()
        {
            string url = $"https://api.qq.jsososo.com/song/urls?id={Id}";
            byte[] data = HttpWebClient.Get(url);
            JObject json = JObject.Parse(Encoding.UTF8.GetString(data));
            return json["data"][Id].ToString();
        }

        public TencentMusic(IMusic music)
        {
            Id = music.Id;
            Name = music.Name;
            Album = music.Album;
            Artists = music.Artists;
            CoverId = music.CoverId;
        }

        public TencentMusic() { }
    }
    public class TencentSinger { public string name { get; set; }}
}
