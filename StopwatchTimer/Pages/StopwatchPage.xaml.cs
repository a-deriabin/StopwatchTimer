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
    /// Interaction logic for StopwatchPage.xaml
    /// </summary>
    public partial class StopwatchPage : Page
    {
        private StopwatchLogic stopwatch = new StopwatchLogic();
        private DispatcherTimer updateTimer = null;

        public StopwatchPage()
        {
            InitializeComponent();

            // Prepare UI
            LoadSavedTimes();
            UpdateDeleteBtnLooks();

            // Prepare timer
            updateTimer = new DispatcherTimer();
            updateTimer.Interval = new TimeSpan(0, 0, 0, 0, 30);
            updateTimer.Tick += delegate (object sender, EventArgs e)
            {
                UpdateTimeText();
            };
        }

        //---------------------
        // UI action handlers

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            stopwatch.Start();
            updateTimer.Start();
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            stopwatch.Stop();
            updateTimer.Stop();
            UpdateTimeText();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            stopwatch.Clear();
            UpdateTimeText();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string timeStr = CurTimeToInlineStr();
            SavedData.AddSavedTime(timeStr);
            LoadSavedTimes();
        }

        private void BtnDeleteSave_Click(object sender, RoutedEventArgs e)
        {
            int id = SavesList.SelectedIndex;
            SavedData.RemoveSavedTime(id);
            LoadSavedTimes();
        }

        private void SavesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDeleteBtnLooks();
        }

        //------------------------
        // Internal logic

        private string CurTimeToInlineStr()
        {
            var time = stopwatch.Time;
            return $"{time.Hours}:{time.Minutes}:{time.Seconds}:{FormatMS(time.Milliseconds)}";
        }

        private void UpdateTimeText()
        {
            var time = stopwatch.Time;

            TimeTextBlock.Text = time.Hours + " Hours\n" +
                time.Minutes + " Minutes\n" +
                time.Seconds + " Seconds\n" +
                FormatMS(time.Milliseconds) + " ms.";
        }

        private string FormatMS(int ms)
        {
            if (ms < 10)
                return "00" + ms;
            if (ms < 100)
                return "0" + ms;
            return ms.ToString();
        }

        private void LoadSavedTimes()
        {
            var times = SavedData.GetAllSavedTimes();

            SavesList.Items.Clear();
            int i = 1;
            foreach (var time in times)
            {
                SavesList.Items.Add($"{i}. {time}");
                i += 1;
            }
        }

        private void UpdateDeleteBtnLooks()
        {
            bool enableBtn = (SavesList.SelectedIndex >= 0 &&
                SavesList.SelectedIndex < SavesList.Items.Count);
            BtnDeleteSave.IsEnabled = enableBtn;
        }
    }
}
