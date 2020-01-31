using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Threading;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace StopwatchTimer.Pages
{
    /// <summary>
    /// Interaction logic for TimerPageNew.xaml
    /// </summary>
    public partial class TimerPageNew : Page, IClosable, IFocusable
    {
        private TimerLogic timerLogic = new TimerLogic();
        private bool didInit = false;

        private DispatcherTimer updateTimer = null;
        private Notifier notifier = null;

        private IChangeableIcon parentWindow;

        public TimerPageNew(IChangeableIcon parentWindow, Window _parentWindow)
        {
            InitializeComponent();
            this.parentWindow = parentWindow;

            // UI updating timer
            updateTimer = new DispatcherTimer();
            updateTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            updateTimer.Tick += delegate (object sender, EventArgs e)
            {
                UpdateTime();
                if (timerLogic.IsFininshed)
                    TimerFinished();
            };

            // For events
            _parentWindow.KeyUp += delegate (object sender, KeyEventArgs e)
            {
                if (e.Key == Key.Enter)
                {
                    if (_BtnStart.IsEnabled)
                    {
                        _BtnStart.Focus();
                        _BtnStart_Click(sender, e);
                    }
                    else
                        _BtnStop_Click(sender, e);
                }
            };

            // For Windows toast notifications
            notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new PrimaryScreenPositionProvider(Corner.BottomRight, 0, 0);

                //cfg.LifetimeSupervisor = new CountBasedLifetimeSupervisor(
                //    MaximumNotificationCount.FromCount(5));

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(10000),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });

            // Done initializing UI
            didInit = true;
        }

        //---------------------
        // UI action handlers

        private void TxtInputGotFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox))
                return;
            var txtBox = sender as TextBox;

            if (txtBox.Text == "00" || txtBox.Text == "0")
                txtBox.Text = "";
        }

        private void TxtInputLostFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox))
                return;
            var txtBox = sender as TextBox;

            if (string.IsNullOrWhiteSpace(txtBox.Text) || txtBox.Text == "0")
                txtBox.Text = "00";
            else if (txtBox.Text.Length == 1)
                txtBox.Text = "0" + txtBox.Text;
        }

        private void TxtInputTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is TextBox))
                return;
            var txtBox = sender as TextBox;

            var oldCaretPos = txtBox.SelectionStart;
            txtBox.Text = Regex.Replace(txtBox.Text, "[^0-9]", "");
            txtBox.SelectionStart = Math.Min(oldCaretPos, txtBox.Text.Length);
            txtBox.SelectionLength = 0;
        }

        private void _TxtHours_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!didInit)
                return;

            bool isLastCol = (_TxtHours.Text.Length > 0 && _TxtHours.Text.Last() == ':');
            TxtInputTextChanged(sender, e);
            if (isLastCol || _TxtHours.Text.Length == 2)
                _TxtMinutes.Focus();
        }

        private void _TxtMinutes_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!didInit)
                return;

            bool isLastCol = (_TxtMinutes.Text.Length > 0 && _TxtMinutes.Text.Last() == ':');
            TxtInputTextChanged(sender, e);
            if (isLastCol || _TxtMinutes.Text.Length == 2)
                _TxtSeconds.Focus();
        }

        private void _BtnStart_Click(object sender, RoutedEventArgs e)
        {
            int hours = int.Parse(_TxtHours.Text);
            int minutes = int.Parse(_TxtMinutes.Text);
            int seconds = int.Parse(_TxtSeconds.Text);

            TimerEnabled(hours, minutes, seconds);
            FinishNotify();
        }

        private void _BtnStop_Click(object sender, RoutedEventArgs e)
        {
            TimerDisabled();
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            FinishNotify();
        }

        private void _TxtHours_KeyUp(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Enter)
            //{
            //    if (_BtnStart.IsEnabled)
            //    {
            //        _BtnStart.Focus();
            //        _BtnStart_Click(sender, e);
            //    }
            //    else
            //        _BtnStop_Click(sender, e);
            //}
        }

        private void _TxtMinutes_KeyUp(object sender, KeyEventArgs e)
        {
            _TxtHours_KeyUp(sender, e);
        }

        private void _TxtSeconds_KeyUp(object sender, KeyEventArgs e)
        {
            _TxtHours_KeyUp(sender, e);
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            
        }

        //----------------------
        // Internal logic

        private void UpdateTime()
        {
            var timeLeft = timerLogic.TimeLeft;

            // Round to larger
            if (timeLeft.Milliseconds > 0)
                timeLeft += new TimeSpan(0, 0, 1);

            _TxtHours.Text = FormatNum(timeLeft.Hours);
            _TxtMinutes.Text = FormatNum(timeLeft.Minutes);
            _TxtSeconds.Text = FormatNum(timeLeft.Seconds);
        }

        private string FormatNum(int timeNum)
        {
            if (timeNum == 0)
                return "00";
            if (timeNum < 10)
                return "0" + timeNum;
            return timeNum.ToString();
        }

        private void TimerFinished()
        {
            TimerDisabled();
            Notify();
        }

        private void SetInputEnabled(bool isEnabled)
        {
            _TxtHours.Focusable = isEnabled;
            _TxtHours.IsReadOnly = !isEnabled;

            _TxtMinutes.Focusable = isEnabled;
            _TxtMinutes.IsReadOnly = !isEnabled;

            _TxtSeconds.Focusable = isEnabled;
            _TxtSeconds.IsReadOnly = !isEnabled;
        }

        private void TimerEnabled(int hours, int minutes, int seconds)
        {
            timerLogic.Start(hours, minutes, seconds);
            updateTimer.Start();

            _BtnStart.IsEnabled = false;
            _BtnStop.IsEnabled = true;
            SetInputEnabled(false);
        }

        private void TimerDisabled()
        {
            updateTimer.Stop();
            timerLogic.Stop();
            UpdateTime();

            _BtnStart.IsEnabled = true;
            _BtnStop.IsEnabled = false;
            SetInputEnabled(true);
        }

        private bool isNotifyShown = false;

        private void Notify()
        {
            notifier.ShowInformation("Finished");
            parentWindow.ChangeIcon("Assets/icon_notify.png");
            isNotifyShown = true;
        }

        private void FinishNotify()
        {
            if (!isNotifyShown)
                return;

            if (notifier != null)
                notifier.ClearMessages();
            parentWindow.ChangeIcon("Assets/icon_simple.png");
            isNotifyShown = false;
        }

        //--------------
        // Interface implementations

        void IClosable.OnAppClosed()
        {
            if (notifier != null)
                notifier.Dispose();
        }

        void IFocusable.OnGotFocus()
        {
            FinishNotify();
        }

        void IFocusable.OnLostFocus()
        {
            // Do nothing
        }

    }
}
