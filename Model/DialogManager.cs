using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeMusic.View;

namespace WeMusic.Model
{
    class DialogManager
    {
        public async static Task<object> ShowCreateListDialog()
        {
            return await DialogHost.Show(new CreateListDialog());
        }

        public async static Task<object> ShowDownloadDialog(object parameter)
        {
            return await DialogHost.Show(new DownloadDialog(parameter));
        }

        public async static Task<object> ShowPlatformListDialog(object parameter)
        {
            return await DialogHost.Show(new PlatformListDialog());
        }
    }
}
