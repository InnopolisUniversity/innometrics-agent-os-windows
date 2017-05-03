using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CommonModels
{
    // написать про то, что он должен создаваться и работать только в фоне
    public class Guard
    {
        private readonly Timer _timer;

        public Guard(Action actionToDoEveryTick, int secondsToCountdown)
        {
            _timer = new Timer(secondsToCountdown * 1000);
            _timer.Elapsed += (sender, args) =>
            {
                actionToDoEveryTick?.Invoke();
            };
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Stop();
        }

        /// <summary>Resets when started</summary>
        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        /// <summary> Start the timer from 0</summary>
        public void Reset()
        {
            _timer.Stop();
            _timer.Start();
        }
    }
}
