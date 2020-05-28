using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WeMusic.Interface;
using WeMusic.ViewModel;

namespace WeMusic.View
{
    /// <summary>
    /// DownloadDialog.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadDialog : UserControl
    {
        public DownloadDialog(object parameter)
        {
            InitializeComponent();
            if (parameter is IMusic)
            {
                (DataContext as DownloadDialogViewModel).Musics.Add(parameter as IMusic);
            }
            else
            {
                (DataContext as DownloadDialogViewModel).Musics = new System.Collections.ObjectModel.ObservableCollection<IMusic>(parameter as IEnumerable<IMusic>);
            }
        }
    }
}
