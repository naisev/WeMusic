using SqlSugar;
using WeMusic.Enum;
using WeMusic.Interface;
using WeMusic.Model.MusicModel;

namespace WeMusic.Model.DbModel
{
    [SugarTable("MusicInfo")]
    public class MusicInfoModel : IMusic
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }
        public string Artists { get; set; }
        public string Name { get; set; }
        public string Album { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string SourceName { get; set; }
        public MusicSource Origin { get; set; }
        public string CoverId { get; set; }

        public MusicInfoModel(IMusic music)
        {
            Id = music.Id;
            Artists = music.Artists;
            Name = music.Name;
            Album = music.Album;
            SourceName = music.SourceName;
            Origin = music.Origin;
            CoverId = music.CoverId;
        }
        public MusicInfoModel() { }

        public IMusic ToIMusic()
        {
            switch (Origin)
            {
                case MusicSource.Kugou:
                    return new KugouMusic(this);
                case MusicSource.Netease:
                    return new NeteaseMusic(this);
                case MusicSource.Tencent:
                    return new TencentMusic(this);
                default:
                    return new LocalMusic(this);
            }
        }
    }
}
