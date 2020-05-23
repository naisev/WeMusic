using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeMusic.Model.DbModel
{
    [SugarTable("CustomList")]
    public class CustomListModel
    {
        public string Title { get; set; }

        public string Id { get; set; }
        public CustomListModel()
        {

        }
        public CustomListModel(string title,string id)
        {
            Title = title;
            Id = id;
        }
    }
}
