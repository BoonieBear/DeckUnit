using System;
using System.Collections.Generic;

namespace BoonieBear.DeckUnit.TaskSchedular.Schedular
{
    public class Executor
    {
        List<Task> _queue = new List<Task>();
        ITimer _timer;

        public Executor(ITimer timer)
        {
            _timer = timer;
            _timer.Register(_timer_Tick);
        }

        private void _timer_Tick()
        {
            //Logger.Log("_timer_Tick start");
            _timer.Stop();
            //Logger.Log("_queue.Count" + _queue.Count);
            if (_queue.Count > 0)
            {
                foreach (Task obj in _queue)
                {
                    //ToDo: check time stamp
                    //Logger.Log("obj.Execution " + obj.Execution.Method.ToString());
                    obj.Execution();
                }
            }
            _timer.Start();
        }

        public void Register(Task t)
        {
            if (t.Interval.TotalMilliseconds == 0)
            {
                (_timer as DispatcherTimerProxy).BeginInvoke(t.Execution);
            }
            else
            {
                _queue.Add(t);
                changeTimerInterval();
            }
        }

        public void UnRegister(Task t)
        {
            _queue.RemoveAll(t1 => t.Execution == t1.Execution);
            changeTimerInterval();
        }

        private void changeTimerInterval()
        {
            _timer.Stop();

            if (_queue.Count <= 0)
            {
                return;
            }

            TimeSpan min = _queue[0].Interval;

            foreach (Task obj in _queue)
            {
                if (obj.Interval < min)
                {
                    min = obj.Interval;
                }
            }          

            _timer.Interval = min;

            if (_queue.Count > 0)
            {
                _timer.Start();
            }
        }
    }
}
