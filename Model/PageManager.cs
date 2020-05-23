using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WeMusic.View;

namespace WeMusic.Model
{
    public class PageManager
    {
        public static SearchPage SearchPage = new SearchPage();
        public static BasePage BasePage = new BasePage();
        public static LyricPage LyricPage = new LyricPage();
        public static Page CurrentPage = BasePage;
    }
}
