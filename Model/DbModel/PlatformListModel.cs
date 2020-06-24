using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeMusic.Model.DbModel
{
    [SugarTable("PlatformList")]
    public class PlatformListModel
    {
        public string PlatformId { get; set; }

        public string MusicId { get; set; }

        public PlatformListModel()
        {

        }
    }
}
