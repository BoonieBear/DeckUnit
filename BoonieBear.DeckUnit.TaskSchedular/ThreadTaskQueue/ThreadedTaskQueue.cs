using System;
using System.Collections.Generic;
using System.Threading;
using BoonieBear.DeckUnit.Utilities.FileLogger;

namespace BoonieBear.DeckUnit.Utilities.ThreadTaskQueue
{
    public class ThreadedTaskQueue:IDisposable
    {
        private List<ThreadCommand> _queue = new List<ThreadCommand>();
        private readonly object _queueSyncObject = new object();

        private Thread _processTaskThread = null;
        private ManualResetEvent _existTaskEvent = new ManualResetEvent(false);
        private ManualResetEvent _exitThreadEvent = new ManualResetEvent(false);
        private static ILogService _logger = new FileLogService(typeof(ThreadedTaskQueue));
        public ThreadedTaskQueue(ThreadPriority priority)
        {
            _existTaskEvent.Reset();
            _exitThreadEvent.Reset();

            _processTaskThread = new Thread(new ThreadStart(ProcessTaskThreadMain));
            _processTaskThread.IsBackground = true;
            _processTaskThread.Priority = ThreadPriority.Normal;
            _processTaskThread.Start();
        }

        private static object _lockObject = new object();
        private static ThreadedTaskQueue _Instance;
        public static ThreadedTaskQueue Instance
        {
            get 
            {
                if (_Instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new ThreadedTaskQueue(ThreadPriority.Normal);   
                        }
                    }  
                }
                return _Instance; 
            }
        }
        

        public ThreadedTaskQueue()
            : this(ThreadPriority.BelowNormal)
        { }

        public void Suspend()
        {
            if (_processTaskThread != null)
            {
                _processTaskThread.Suspend();
            }
        }

        public void Resume()
        {
            if (_processTaskThread != null)
            {
                _processTaskThread.Resume();
            }
        }

        private void ProcessTaskThreadMain()
        {
            bool hasWorked = false;

            while (true)
            {
                if (hasWorked == true)
                {
                    try
                    {
                        CheckExistenceOfQueueContents();
                    }
                    catch (Exception ex)
                    {
                        _logger.Fatal(null, ex);
                    }
                }

                _existTaskEvent.WaitOne();

                if (_exitThreadEvent.WaitOne(0, false))
                {
                    return;
                }

                ThreadCommand task = Dequeue();
                if (task != null)
                {
                    try
                    {
                        ProcessOverride(task);
                    }
                    catch(Exception ex)
                    {
                        task.State = COMMAND_STATE.STATE_CANCEL;
                        _logger.Fatal(null, ex);
                    }

                    hasWorked = true;
                }
            }
        }

        private void ProcessOverride(ThreadCommand task)
        {
            if (task.Executed == null)
            {
                task.State = COMMAND_STATE.STATE_CANCEL;
                return;
            }
            if ((task.CanExecuted != null))
            {
                bool canExecuted = false;
                if (task.IsUseDispatcher)
                {
                    if (task.CallBackDispather != null)
                    {
                        canExecuted = (bool)task.CallBackDispather.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                            task.CanExecuted, task.Parameter);
                    }
                }
                else
                {
                    canExecuted = task.CanExecuted(task.Parameter);
                }
                if (!canExecuted)
                {
                    task.State = COMMAND_STATE.STATE_CANCEL;
                    return;
                }
            }
            try
            {
                task.Executed(task.Parameter);
            }
            catch (Exception ex)
            {
                _logger.Fatal(null, ex);
            }
            
            if ((task.Complete != null))
            {
                if (task.IsUseDispatcher)
                {
                    if (task.CallBackDispather != null)
                    {
                        task.CallBackDispather.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                            task.Complete, task.Parameter);
                    }
                }
                else
                {
                    task.Complete(task.Parameter);
                }
            }
            task.State = COMMAND_STATE.STATE_COMPLETE;
        }

        private ThreadCommand Dequeue()
        {
            lock (_queueSyncObject)
            {
                ThreadCommand result = null;
                if (_queue.Count >= 1)
                {
                    result = _queue[0];
                    _queue.RemoveAt(0);
                    result.State = COMMAND_STATE.STATE_RUN;
                }

                bool eventFlag = _existTaskEvent.WaitOne(0, false);

                if ((eventFlag == true) && (_queue.Count == 0))
                {
                    _existTaskEvent.Reset();
                }

                return result;
            }
        }

        public bool Enqueue(ThreadCommand task, THREADED_ENQUEUE_TYPE enqueueType = THREADED_ENQUEUE_TYPE.CLEAR_AND_ADD)
        {
            if (task == null)
            {
                return false;
            }

            if (task != null)
            {
                lock (_queueSyncObject)
                {
                    switch (enqueueType)
                    {
                        case THREADED_ENQUEUE_TYPE.NORMAL:
                            _queue.Add(task);
                            task.State = COMMAND_STATE.STATE_WAIT;
                            break;
                        case THREADED_ENQUEUE_TYPE.CLEAR_AND_ADD:
                            for (int i = 0; i < _queue.Count; i++)
                            {
                                if (_queue[i].CommandID == task.CommandID)
                                {
                                    _queue[i].State = COMMAND_STATE.STATE_CANCEL;
                                    _queue.RemoveAt(i);
                                    i--;
                                }
                            }
                            _queue.Add(task);
                            task.State = COMMAND_STATE.STATE_WAIT;
                            break;
                        default:
                            break;
                    }
                    
                    bool eventFlag = _existTaskEvent.WaitOne(0, false);

                    if ((eventFlag == false) && (_queue.Count > 0))
                    {
                        _existTaskEvent.Set();
                    }
                }
            }

            return true;
        }

        public virtual ThreadCommand PreprocessEnqueuingTask(ThreadCommand task,out THREADED_ENQUEUE_TYPE enqueueType)
        {
            enqueueType = THREADED_ENQUEUE_TYPE.NORMAL;
            return task;
        }

        private void CheckExistenceOfQueueContents()
        {
            int count = 0;

            lock (_queueSyncObject)
            {
                count = _queue.Count;
            }

            if (count == 0)
            {
                EnqueuedTaskCompleted();
            }
        }

        public virtual void EnqueuedTaskCompleted()
        { }


        #region IDisposable Members

        public void Dispose()
        {
            _exitThreadEvent.Set();
            _existTaskEvent.Set();

            try
            {
                if (_processTaskThread.Join(1000) == false)
                {
                    _processTaskThread.Abort();
                }
            }
            catch(Exception ex)
            {
                _logger.Fatal(null, ex);
            }
        }

        #endregion
    }
}
