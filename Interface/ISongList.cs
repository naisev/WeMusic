using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeMusic.Enum;

namespace WeMusic.Interface
{
    public interface ISongList
    {
        public string Title { get; set; }

        public IMusic[] Musics { get; set; }

        public string CoverUrl { get; set; }

        public string Id { get; set; }

        public MusicSource Origin { get; }
        
        public string SourceName { get; }
    }
}
