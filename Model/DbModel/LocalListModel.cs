using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeMusic.Model.DbModel
{
    [SugarTable("LocalList")]
    public class LocalListModel
    {
        public string FilePath { get; set; }
    }
}
