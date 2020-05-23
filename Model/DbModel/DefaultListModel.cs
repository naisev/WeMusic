using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeMusic.Model.DbModel
{
    [SugarTable("DefaultList")]
    public class DefaultListModel
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }
        public DefaultListModel()
        {

        }
        public DefaultListModel(string id)
        {
            Id = id;
        }
    }
}
