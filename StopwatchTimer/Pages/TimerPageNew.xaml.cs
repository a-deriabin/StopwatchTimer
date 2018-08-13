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

namespace StopwatchTimer.Pages
{
    /// <summary>
    /// Interaction logic for TimerPageNew.xaml
    /// </summary>
    public partial class TimerPageNew : Page, IPausable
    {
        private bool didInit = false;

        public TimerPageNew()
        {
            InitializeComponent();
            didInit = true;
        }

        void IPausable.ContinueActions()
        {
            //throw new NotImplementedException();
        }

        void IPausable.PauseActions()
        {
            //throw new NotImplementedException();
        }

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
    }
}
