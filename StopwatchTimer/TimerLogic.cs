using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StopwatchTimer
{
    sealed class TimerLogic
    {
        private DateTime whenToStop = default(DateTime);
        private DateTime whenStopped = default(DateTime);

        public bool IsStarted { get; private set; } = false;

        public TimeSpan TimeLeft
        {
            get
            {
                var timeLeft = (IsStarted) ? (whenToStop - DateTime.Now)
                                           : (whenToStop - whenStopped);

                // Ensure we will never return negative time
                return (timeLeft.TotalMilliseconds <= 0) ? TimeSpan.Zero : timeLeft;
            }
        }

        public bool IsFininshed
        {
            get { return (TimeLeft.TotalMilliseconds <= 0); }
        }

        public void Start(int hours, int minutes, int seconds)
        {
            whenToStop = DateTime.Now + new TimeSpan(hours, minutes, seconds);
            IsStarted = true;
        }

        public void Stop()
        {
            whenStopped = DateTime.Now;
            IsStarted = false;
        }


    }
}
