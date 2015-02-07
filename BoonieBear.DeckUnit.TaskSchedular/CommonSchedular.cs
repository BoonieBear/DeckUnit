using System;
using System.Threading;
using BoonieBear.DeckUnit.TaskSchedular.Schedular;

namespace BoonieBear.DeckUnit.TaskSchedular
{
    //Todo : there multiple schedular, try to remove them
    class CommonSchedular
    {
        private static CommonSchedular _schedular;
        private static bool _isInstantiated = false;
        private static object lockObj = new object();

        private Executor _executor;

        public static CommonSchedular GetInstance()
        {
            if (!_isInstantiated)
            {
                try
                {
                    Monitor.Enter(lockObj);
                    if (!_isInstantiated)
                    {
                        _schedular = new CommonSchedular();
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

        private CommonSchedular()
        {
            TimerProxy timerProxy = new TimerProxy();
            _executor = new Executor(timerProxy);
        }

        public void Schedule(TaskExecution execution)
        {
            Task task = new Task(new TimeSpan(0, 0, 0,1), execution);
            _executor.Register(task);
        }
    }
}
