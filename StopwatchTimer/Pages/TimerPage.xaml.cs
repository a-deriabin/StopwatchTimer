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
using System.Windows.Threading;

namespace StopwatchTimer.Pages
{
    /// <summary>
    /// Interaction logic for TimerPage.xaml
    /// </summary>
    public partial class TimerPage : Page, IPausable
    {
        private InputLogic inputLogic = new InputLogic();
        private DispatcherTimer updateTimer = null;

        public TimerPage()
        {
            InitializeComponent();

            // Update UI
            _TxtLeftTime.Text = "00:00:00";
            _TxtFinishTime.Text = "00:00:00";

            // Prepare timer
            updateTimer = new DispatcherTimer();
            updateTimer.Tick += delegate (object sender, EventArgs e)
            {
                UpdateTimerTick();
            };
            updateTimer.Start();
        }

        //---------------------
        // UI action handlers

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void _TxtFinishTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            inputLogic.TextChanged(sender);
        }

        private void _TxtFinishTime_SelectionChanged(object sender, RoutedEventArgs e)
        {
            inputLogic.SelectionChanged(sender);
        }

        private void _TxtFinishTime_LostFocus(object sender, RoutedEventArgs e)
        {
            inputLogic.LostFocus(sender);
        }

        private void _TxtLeftTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            inputLogic.SelectionChanged(sender);

            if (!inputLogic.HasTime(_TxtLeftTime))
                return;

            TimeSpan time = inputLogic.GetTime(_TxtLeftTime);
            DateTime endTime = DateTime.Now + time;
            _TxtFinishTime.Text = endTime.Hour.ToString("00") + ":" +
                        endTime.Minute.ToString("00") + ":" +
                        endTime.Second.ToString("00");
        }

        private void _TxtLeftTime_SelectionChanged(object sender, RoutedEventArgs e)
        {
            inputLogic.SelectionChanged(sender);
        }

        private void _TxtLeftTime_LostFocus(object sender, RoutedEventArgs e)
        {
            inputLogic.SelectionChanged(sender);
        }

        //---------------------
        // Internal logic

        private void UpdateTimerTick()
        {
            _TimeTextBlock.Text = DateTime.Now.Hour.ToString("00") + ":" +
                            DateTime.Now.Minute.ToString("00") + ":" +
                            DateTime.Now.Second.ToString("00");
        }


        //-----------------------------
        // IPausable implementation



        public void PauseActions()
        {
            
        }

        public void ContinueActions()
        {
            
        }

        
    }
}
