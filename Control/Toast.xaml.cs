using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WeMusic.Control
{
    /// <summary>
    /// Toast.xaml 的交互逻辑
    /// </summary>
    public partial class Toast : UserControl
    {
        private StyleInfo[] StyleCollection = StyleInfo.LoadLists();
        public enum InfoType
        {
            Success = 0,
            Info = 1,
            Warning = 2,
            Error = 3
        }

        public Toast(string content,InfoType showStyle)
        {
            InitializeComponent();
            Content.Content = content;
            TheBorder.Background = new SolidColorBrush(StyleCollection[(int)showStyle].BgColor);
            Icon.Data = Geometry.Parse(StyleCollection[(int)showStyle].PathStr);
        }

        public void WillRemove()
        {
            DoubleAnimation daV = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(1)));
            this.BeginAnimation(OpacityProperty, daV);
        }
        
        class StyleInfo
        {
            public Color BgColor { get; }
            public string PathStr { get; }
            public StyleInfo(Color color,string pathStr)
            {
                BgColor = color;
                PathStr = pathStr;
            }
            public static StyleInfo[] LoadLists()
            {
                StyleInfo[] infos = new StyleInfo[4];
                infos[0] = new StyleInfo(Color.FromRgb(17, 173, 69), "M21,7L9,19L3.5,13.5L4.91,12.09L9,16.17L19.59,5.59L21,7Z");
                infos[1] = new StyleInfo(Color.FromRgb(20, 126, 201), "M11,9H13V7H11M12,20C7.59,20 4,16.41 4,12C4,7.59 7.59,4 12,4C16.41,4 20,7.59 20,12C20,16.41 16.41,20 12,20M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2M11,17H13V11H11V17Z");
                infos[2] = new StyleInfo(Color.FromRgb(245, 163, 0), "M12,2L1,21H23M12,6L19.53,19H4.47M11,10V14H13V10M11,16V18H13V16");
                infos[3] = new StyleInfo(Color.FromRgb(230, 9, 20), "M11,15H13V17H11V15M11,7H13V13H11V7M12,2C6.47,2 2,6.5 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2M12,20A8,8 0 0,1 4,12A8,8 0 0,1 12,4A8,8 0 0,1 20,12A8,8 0 0,1 12,20Z");
                return infos;
            }
        }

        public static void Show(string content, InfoType showStyle, int showTime = 1500)
        {

            //计算Show 位置
            double left;
            double top;
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                left = SystemParameters.WorkArea.Width / 2 - 125;
                top = SystemParameters.WorkArea.Height * 0.75;
            }
            else
            {
                left = Application.Current.MainWindow.Left + Application.Current.MainWindow.ActualWidth / 2 - 125;
                top = Application.Current.MainWindow.Top + Application.Current.MainWindow.ActualHeight * 0.75;
            }

            //创建新窗口并Show
            Window w = new Window();
            w.Height = 50;
            w.Width = 250;
            w.WindowStyle = WindowStyle.None;
            w.AllowsTransparency = true;
            w.Background = null;
            Toast toast = new Toast(content, showStyle);
            w.Content = toast;
            w.Left = left;
            w.Top = top;
            w.Topmost = true;


            System.Timers.Timer t = new System.Timers.Timer(showTime);
            t.Elapsed += new ElapsedEventHandler(delegate (object obj, ElapsedEventArgs e)
            {
                w.Dispatcher.Invoke(new Action(delegate
                {
                    toast.WillRemove();
                    System.Timers.Timer t2 = new System.Timers.Timer(1000);
                    t2.Elapsed += new ElapsedEventHandler(delegate (object obj2, ElapsedEventArgs e2)
                    {
                        w.Dispatcher.Invoke(new Action(delegate
                        {
                            w.Close();
                        }));
                    });
                    t2.AutoReset = false;
                    t2.Start();
                }));
            });
            t.AutoReset = false;
            t.Start();
            w.Show();
        }

    }
}
