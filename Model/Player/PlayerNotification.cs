using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WeMusic.Enum;
using WeMusic.ViewModel;

namespace WeMusic.Model.Player
{
    public static class PlayerNotification
    {
        private static DispatcherTimer timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(0.2) };
        static PlayerNotification()
        {
            timer.Tick += NotificationEvent;
        }
        public static void Start()
        {
            if (timer.IsEnabled) { return; }
            timer.Stop();
            timer.Start();
        }
        public static void Stop()
        {
            timer.Stop();
        }
        private static void NotificationEvent(object sender, EventArgs e)
        {
            ViewModelManager.MainWindowViewModel.MusicNowTime = PlayerManager.Position;
            ViewModelManager.MainWindowViewModel.MusicMaxTime = PlayerManager.Length;

            if (PlayerManager.State == NAudio.Wave.PlaybackState.Stopped)
            {
                try
                {
                    PlayerManager.PlayMusic = PlayerList.Next();
                    PlayerManager.Play();
                }
                catch (Exception)
                {
                    PlayerManager.Stop();
                }
            }
        }
    }
}
