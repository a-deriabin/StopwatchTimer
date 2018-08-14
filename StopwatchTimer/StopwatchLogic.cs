using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StopwatchTimer
{
    sealed class StopwatchLogic
    {
        private DateTime whenStarted = default(DateTime);
        private DateTime whenStopped = default(DateTime);

        public bool IsStarted { get; private set; } = false;

        public TimeSpan Time
        {
            get
            {
                return (IsStarted) ? (DateTime.Now - whenStarted)
                                   : (whenStopped - whenStarted);
            }
        }

        public void Start()
        {
            if (IsStarted)
                return;

            var elapsedTime = whenStopped - whenStarted;
            whenStarted = DateTime.Now - elapsedTime;
            IsStarted = true;
        }

        public void Stop()
        {
            if (!IsStarted)
                return;

            whenStopped = DateTime.Now;
            IsStarted = false;
        }

        public void Clear()
        {
            whenStarted = DateTime.Now;
            whenStopped = DateTime.Now;
        }

    }
}
