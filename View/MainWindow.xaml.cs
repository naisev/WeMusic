using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using WeMusic.Model;
using WeMusic.ViewModel;

namespace WeMusic
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            MaxHeight = SystemParameters.WorkArea.Height + 16;
        }
    }
}
