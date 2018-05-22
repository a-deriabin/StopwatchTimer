using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace StopwatchTimer.Pages
{
    public partial class TimerPage : Page, IPausable
    {
        protected partial class InputLogic
        {
			/// <summary>
            /// Is used to mark objects as locked/unlocked.
            /// </summary>
            private sealed class LockSystem
            {
                private HashSet<object> locked = new HashSet<object>();

                public void SetLock(object obj, bool state)
                {
                    lock (locked)
                    {
                        if (state && !locked.Contains(obj))
                            locked.Add(obj);
                        if (!state && locked.Contains(obj))
                            locked.Remove(obj);
                    }
                }

                public bool GetLock(object obj)
                {
                    lock (locked)
                        return locked.Contains(obj);
                }
            }
        }
    }
}
