using WeMusic.Enum;

namespace WeMusic.Interface
{
    public interface IMusic
    {
        /// <summary>
        /// 音乐链接ID
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// 封面id，只有网易云需要，其他音乐默认为ID
        /// </summary>
        string CoverId { get; set; }

        /// <summary>
        /// 用于正确产生接口产生的艺术家数组名称
        /// </summary>
        string Artists
        {
            get;
            set;
        }

        /// <summary>
        /// 音乐名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 音乐专辑
        /// </summary>
        string Album { get; set; }

        /// <summary>
        /// 音乐来源
        /// </summary>
        string SourceName { get; }

        /// <summary>
        /// 音乐来源
        /// </summary>
        MusicSource Origin { get; }
    }
}
