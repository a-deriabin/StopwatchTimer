using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace StopwatchTimer.Pages
{
    /// <summary>
    /// Interaction logic for TimerPage.xaml
    /// </summary>
    public partial class TimerPage : Page, IPausable
    {
        protected partial class InputLogic
        {
            private LockSystem locks = new LockSystem();

            public void TextChanged(object sender)
            {
                if (!(sender is TextBox))
                    throw new ArgumentException("Unsupported argument type.");
                TextBox txtBox = sender as TextBox;

                if (locks.GetLock(txtBox))
                    return;

                StringBuilder sb = new StringBuilder();
                int curSector = 0;

                // Append only valid chars
                for (int i = 0; i < txtBox.Text.Length; i++)
                {
                    char c = txtBox.Text[i];                    
                    
                    if (char.IsDigit(c))
                        sb.Append(c);

                    // Only first 2 colons are valid
                    if (c == ':' && curSector < 2)
                    {
                        curSector += 1;
                        sb.Append(c);
                    }
                }

                // Fix too long sectors
                int[] MAX_SECTOR_LEN = { 3, 2, 2 };
                for (int i = 0; i < 3; i++)
                {
                    int len = GetSectorText(sb.ToString(), i).Length;
                    if (len > MAX_SECTOR_LEN[i])
                    {
                        int sectorStart = GetSectorStartPos(sb.ToString(), i);
                        sb.Remove(sectorStart, len - MAX_SECTOR_LEN[i]);
                    }
                }

                // If were no invalid chars, do nothing
                if (sb.Length == txtBox.Text.Length)
                    return;

                // Replace invalid string with valid one
                locks.SetLock(txtBox, true);
                int selStart = txtBox.SelectionStart;
                txtBox.Text = sb.ToString();
                txtBox.SelectionStart = Math.Min(selStart, txtBox.Text.Length);
                locks.SetLock(txtBox, false);
            }

            public void SelectionChanged(object sender)
            {
                if (!(sender is TextBox))
                    throw new ArgumentException("Unsupported argument type.");
                TextBox txtBox = sender as TextBox;

                if (locks.GetLock(txtBox))
                    return;

                // Called many times on program startup
                if (!txtBox.IsFocused)
                {
                    txtBox.Text = FixTimeText(txtBox.Text);
                    return;
                }

                int selIndex = txtBox.SelectionStart;
                if (selIndex < 0)
                    return;

                int sector = GetSelectedSector(txtBox.Text, selIndex);
                locks.SetLock(txtBox, true);

                txtBox.Text = FixTimeText(txtBox.Text);
                string text = GetSectorText(txtBox.Text, sector);

                if (text == "0" || text == "00")
                {
                    txtBox.Text = SetSectorText(txtBox.Text, "", sector);
                    txtBox.SelectionStart = GetSectorClickPos(txtBox.Text, sector);
                    txtBox.SelectionLength = 0;
                }
                locks.SetLock(txtBox, false);
            }

            public void LostFocus(object sender)
            {
                if (!(sender is TextBox))
                    throw new ArgumentException("Unsupported argument type.");
                TextBox txtBox = sender as TextBox;

                txtBox.Text = FixTimeText(txtBox.Text);
                locks.SetLock(txtBox, false);
            }


            public bool HasTime(TextBox txtBox)
            {
                return (GetTime(txtBox) != new TimeSpan(0, 0, 0));
            }

            public TimeSpan GetTime(TextBox txtBox)
            {
                int[] time = { 0, 0, 0 };

                string secTxt;
                for (int i = 0; i <= 2; i++)
                {
                    secTxt = GetSectorText(txtBox.Text, i);
                    if (!string.IsNullOrWhiteSpace(secTxt))
                        time[i] = int.Parse(secTxt);
                }

                return new TimeSpan(time[0], time[1], time[2]);
            }

            /* Internal tools */

            private int GetSelectedSector(string text, int selStart)
            {
                int[] colonIndexes = GetColonIndexes(text);
                return (selStart <= colonIndexes[0]) ? 0 :
                       (selStart > colonIndexes[1]) ? 2 : 1;
            }

            private int GetSectorClickPos(string text, int sector)
            {
                if (sector == 0)
                    return 0;
                if (sector == 1)
                    return GetColonIndexes(text)[0] + 1;
                if (sector == 2)
                    return text.Length;
                throw new ArgumentException("Invalid sector id: " + sector);
            }

            private int GetSectorStartPos(string text, int sector)
            {
                if (sector == 0)
                    return 0;
                if (sector == 1)
                    return GetColonIndexes(text)[0] + 1;
                if (sector == 2)
                    return GetColonIndexes(text)[1] + 1;
                throw new ArgumentException("Invalid sector id: " + sector);
            }

            private string GetSectorText(string text, int sector)
            {
                int[] colonIndexes = GetColonIndexes(text);
                if (sector == 0)
                    return text.Substring(0, colonIndexes[0]);
                if (sector == 1)
                    return text.Substring(colonIndexes[0] + 1, colonIndexes[1] - colonIndexes[0] - 1);
                if (sector == 2)
                    return text.Substring(colonIndexes[1] + 1);

                throw new ArgumentException("Invalid sector id: " + sector);
            }

            private int[] GetSectorLengths(string text)
            {
                string[] sectors = text.Split(':');
                if (sectors.Length != 3)
                    throw new ArgumentException("Expected 3 sectors, found: " + sectors.Length);
                return new int[]
                {
                    sectors[0].Length,
                    sectors[1].Length,
                    sectors[2].Length
                };
            }

            private string SetSectorText(string text, string sectorText, int sector)
            {
                int[] colonIndexes = GetColonIndexes(text);
                if (sector == 0)
                    return Remove(text, 0, colonIndexes[0]).Insert(0, sectorText);
                if (sector == 1)
                    return Remove(text, colonIndexes[0] + 1, colonIndexes[1] - colonIndexes[0] - 1)
                        .Insert(colonIndexes[0] + 1, sectorText);
                if (sector == 2)
                    return Remove(text, colonIndexes[1] + 1).Insert(colonIndexes[1] + 1, sectorText);
                throw new ArgumentException("Invalid sector id: " + sector);
            }

            private string Remove(string text, int from)
            {
                if (from >= text.Length)
                    return text;
                return text.Remove(from);
            }

            private string Remove(string text, int from, int count)
            {
                if (from >= text.Length)
                    return text;
                return text.Remove(from, count);
            }

            private int[] GetColonIndexes(string text)
            {
                int[] colonIndexes = { -1, -1 };
                int curColon = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == ':')
                        colonIndexes[curColon++] = i;
                }

                if (colonIndexes[0] == -1 || colonIndexes[1] == -1)
                    throw new ArgumentException("Invalid input format.");

                return colonIndexes;
            }

            private string FixTimeText(string timeText)
            {
                string secText = "";
                for (int i = 0; i <= 2; i++)
                {
                    secText = GetSectorText(timeText, i);
                    if (secText == "")
                        timeText = SetSectorText(timeText, "00", i);
                }
                return timeText;
            }
        }
    }
}
