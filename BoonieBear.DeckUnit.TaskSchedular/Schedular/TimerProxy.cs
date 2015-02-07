using System;
using System.Timers;

namespace BoonieBear.DeckUnit.TaskSchedular.Schedular
{
    public class TimerProxy : ITimer
    {
        private Timer _timer = new Timer();
        private TimeSpan _interval;
        private TimerEventHandler _handler;

        public TimeSpan Interval
        {
            get
            {
                return _interval;
            }
            set
            {
                _interval = value;
                _timer.Interval = _interval.TotalMilliseconds;
            }
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public void Register(TimerEventHandler Tick)
        {
            _handler = Tick;
            _timer.Elapsed += _timer_Elapsed;
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_handler != null)
            {
                _handler();
            }
        }
    }
}
