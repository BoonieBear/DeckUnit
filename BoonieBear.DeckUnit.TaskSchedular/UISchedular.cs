using System;
using System.Threading;
using BoonieBear.DeckUnit.TaskSchedular.Schedular;
using BoonieBear.DeckUnit.Utilities.Schedular;

namespace BoonieBear.DeckUnit.TaskSchedular
{
    //Todo : there multiple schedular, try to remove them
    public class UISchedular
    {
        private static UISchedular _schedular;
        private static bool _isInstantiated = false;
        private static object lockObj = new object();

        private Executor _executor;

        public static UISchedular GetInstance()
        {
            if (!_isInstantiated)
            {
                try
                {
                    Monitor.Enter(lockObj);
                    if (!_isInstantiated)
                    {
                        _schedular = new UISchedular();
                        _isInstantiated = true;
                    }
                }
                finally
                {
                    Monitor.Exit(lockObj);
                }
                return _schedular;
            }

            return _schedular;
        }

        private UISchedular()
        {
            DispatcherTimerProxy timerProxy = new DispatcherTimerProxy();
            _executor = new Executor(timerProxy);
        }

        /// <summary>
        /// Run a task periodically, default interval 1 second
        /// </summary>
        /// <param name="execution">Task which you want to run</param>
        public void Schedule(TaskExecution execution)
        {
            Task task = new Task(new TimeSpan(0, 0, 0, 1), execution);
            _executor.Register(task);
        }

        /// <summary>
        /// Run a task periodically
        /// </summary>
        /// <param name="execution">Task which you want to run</param>
        /// <param name="intervalSeconds">interval time, unit is second</param>
        public void Schedule(TaskExecution execution, TimeSpan interval)
        {
            Task task = new Task(interval, execution);
            _executor.Register(task);
        }

        /// <summary>
        /// Cancel a period task
        /// </summary>
        /// <param name="execution">Task which you want to cancel</param>
        public void UnSchedule(TaskExecution execution)
        {
            Task task = new Task(new TimeSpan(0, 0, 0, 1), execution);
            _executor.UnRegister(task);
        }

        /// <summary>
        /// Run a task once
        /// </summary>
        /// <param name="execution">Task which you want to run</param>
        public void Execute(TaskExecution execution)
        {
            Task task = new Task(new TimeSpan(0, 0, 0, 0), execution);
            _executor.Register(task);
        }
    }
}