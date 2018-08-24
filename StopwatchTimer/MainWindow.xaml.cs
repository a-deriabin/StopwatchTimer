using StopwatchTimer.Pages;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StopwatchTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IChangeableIcon
    {
        private Page[] pages;

        private Page currentPage = null;
        private PageType curPageType = PageType.Stopwatch;

        public MainWindow()
        {
            InitializeComponent();
            pages = new Page[] { new StopwatchPage(), new TimerPageNew(this) };

            // Prepare UI
            SetPage(PageType.Stopwatch);
            UpdateTopmostState();
        }

        //---------------------
        // UI events

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
            string icon_copyright = "\nUsing a modified version of icon designed by flaticon.com user Smashicons.";

            MessageBox.Show($"{desc}\n{author}\n{version}\n{icon_copyright}", title);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            foreach (var page in pages)
                if (page is IClosable)
                    ((IClosable)page).OnAppClosed();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            foreach (var page in pages)
                if (page is IFocusable)
                    ((IFocusable)page).OnGotFocus();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            foreach (var page in pages)
                if (page is IFocusable)
                    ((IFocusable)page).OnLostFocus();
        }

        //-------------------------------
        // Main window logic

        private void SetPage(PageType page)
        {
            // Navigate to selected page
            Page navPage = PageByType(page);
            _mainFrame.Navigate(navPage);
            curPageType = page;
            currentPage = navPage;

            // Update button in header menu
            if (page == PageType.Stopwatch)
                SwitchBtn.Header = "Switch to _timer";
            else
                SwitchBtn.Header = "Switch to _stopwatch";
        }

        private Page PageByType(PageType type)
        {
            switch (type)
            {
                case PageType.Stopwatch:
                    return pages.Where(p => p is StopwatchPage).First();
                case PageType.Timer:
                    return pages.Where(p => p is TimerPageNew).First();
                default:
                    throw new ArgumentException("Unknown page type: " + type);
            }
        }

        private void UpdateTopmostState()
        {
            this.Topmost = SavedData.IsTopmost;
            TopmostBtn.IsChecked = SavedData.IsTopmost;
        }

        //--------------------------------
        // IChangeableIcon implementation

        void IChangeableIcon.ChangeIcon(string iconPath)
        {
            Uri iconUri = new Uri("pack://application:,,,/" + iconPath, UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);
        }

    }
}
