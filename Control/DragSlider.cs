using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace WeMusic.Control
{
    public class DragSlider : Slider
    {
        private Thumb _thumb = null;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (_thumb != null)
            {
                _thumb.MouseEnter -= thumb_MouseEnter;
            }
            _thumb = (GetTemplateChild("PART_Track") as Track).Thumb;
            if (_thumb != null)
            {
                _thumb.MouseEnter += thumb_MouseEnter;
            }
        }

        private void thumb_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                MouseButtonEventArgs args = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left);
                args.RoutedEvent = MouseLeftButtonDownEvent;
                (sender as Thumb).RaiseEvent(args);
            }
        }
    }
}
