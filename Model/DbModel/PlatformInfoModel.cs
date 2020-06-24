using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeMusic.Enum;
using WeMusic.Interface;
using WeMusic.Model.MusicModel;

namespace WeMusic.Model.DbModel
{
    [SugarTable("PlatformInfo")]
    public class PlatformInfoModel : ISongList
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }

        [SugarColumn(IsIgnore = true)]
        public IMusic[] Musics { get; set; }

        public string CoverUrl { get; set; }

        public MusicSource Origin { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string SourceName { get; set; }

        public string Title { get; set; }

        public PlatformInfoModel()
        {

        }

        public PlatformInfoModel(ISongList songlist)
        {
            Id = songlist.Id;
            Musics = songlist.Musics;
            CoverUrl = songlist.CoverUrl;
            Origin = songlist.Origin;
            SourceName = songlist.SourceName;
            Title = songlist.Title;
        }

        public ISongList ToSongList()
        {
            switch (Origin)
            {
                case MusicSource.Kugou:
                    return new KugouList(this);
                case MusicSource.Netease:
                    return new NeteaseList(this);
                case MusicSource.Tencent:
                    return new TencentList(this);
                default:
                    return null;
            }
        }
    }
}
