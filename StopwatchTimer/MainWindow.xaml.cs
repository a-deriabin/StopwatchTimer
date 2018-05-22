using StopwatchTimer.Pages;
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

namespace StopwatchTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Page stopwatchPage = new StopwatchPage();
        private Page timerPage = new TimerPage();
        private Page currentPage = null;
        private PageType curPageType = PageType.Stopwatch;

        public MainWindow()
        {
            InitializeComponent();

            // Prepare UI
            SetPage(PageType.Stopwatch);
            UpdateTopmostState();
        }

        private void Topmost_Click(object sender, RoutedEventArgs e)
        {
            SavedData.IsTopmost = !SavedData.IsTopmost;
            UpdateTopmostState();
        }

        private void SwitchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (curPageType == PageType.Stopwatch)
                SetPage(PageType.Timer);
            else
                SetPage(PageType.Stopwatch);

            // To fix issue with how button looks after click
            SwitchBtn.Focus();
        }

        private void SwitchBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            // To fix issue with how button looks after click
            this.Focus();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            string title = "About StopwatchTimer";
            string desc = "A simple stopwatch and timer app for Windows.";
            string author = "Author: Andrej Deriabin";
            string version = "Version: " + 
                System.Reflection.Assembly.GetExecutingAssembly()
                .GetName().Version.ToString();

            MessageBox.Show($"{desc}\n{author}\n{version}", title);
        }

        private void SetPage(PageType page)
        {
            if (currentPage != null && currentPage is IPausable)
                ((IPausable)currentPage).PauseActions();

            Page navPage = (page == PageType.Stopwatch) ? stopwatchPage : timerPage;
            _mainFrame.Navigate(navPage);
            curPageType = page;
            currentPage = navPage;

            if (page == PageType.Stopwatch)
                SwitchBtn.Header = "Switch to _timer";
            else
                SwitchBtn.Header = "Switch to _stopwatch";

            if (navPage != null && navPage is IPausable)
                ((IPausable)currentPage).ContinueActions();
        }

        private void UpdateTopmostState()
        {
            this.Topmost = SavedData.IsTopmost;
            TopmostBtn.IsChecked = SavedData.IsTopmost;
        }
    }
}
