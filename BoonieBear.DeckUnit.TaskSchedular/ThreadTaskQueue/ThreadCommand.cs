using System;
using System.Windows.Threading;

namespace BoonieBear.DeckUnit.TaskSchedular.ThreadTaskQueue
{
    /// <summary>
    /// ThreadedTaskQueue used by the command
    /// </summary>
    public class ThreadCommand:IDisposable
    {
        /// <summary>
        /// The implementation of the command
        /// </summary>
        /// <param name="parameter">Executive function parameters</param>
        public delegate void ExecutedThreadEventHandle(object parameter);

        private ExecutedThreadEventHandle _executed;
        public ExecutedThreadEventHandle Executed
        {
            set 
            {
                if (_state == COMMAND_STATE.STATE_NONE)
                {
                    _executed = value;
                }
            }
            get 
            { 
                return _executed; 
            }
        }

        /// <summary>
        /// Ready to check whether
        /// </summary>
        /// <param name="parameter">Executive function parameters</param>
        /// <returns>True is ready</returns>
        public delegate bool CanExecutedThreadEventHandle(object parameter);

        private CanExecutedThreadEventHandle _canExecuted;
        public CanExecutedThreadEventHandle CanExecuted
        {
            set 
            {
                if (_state == COMMAND_STATE.STATE_NONE)
                {
                    _canExecuted = value;
                }
            }
            get 
            { 
                return _canExecuted; 
            }
        }

        /// <summary>
        /// The Executed is Complete
        /// </summary>
        /// <param name="parameter">Executive function parameters</param>
        public delegate void CompleteThreadEventHandle(object parameter);

        private CompleteThreadEventHandle _complete;
        public CompleteThreadEventHandle Complete
        {
            set 
            {
                if (_state == COMMAND_STATE.STATE_NONE)
                {
                    _complete = value;
                }
            }
            get 
            { 
                return _complete; 
            }
        }

        /// <summary>
        /// Executive function parameters
        /// </summary>
        private object _parameter;
        public object Parameter
        {
            set 
            {
                if (_state == COMMAND_STATE.STATE_NONE)
                {
                    _parameter = value;
                }
            }
            get 
            { 
                return _parameter; 
            }
        }

        /// <summary>
        /// Thread Command Type
        /// </summary>
        private string _commandID;
        public string CommandID
        {
            set 
            {
                _commandID = value;
            }
            get { return _commandID; }
        }

        /// <summary>
        /// Command Priority
        /// </summary>
        private COMMAND_PRIORITY _priority;
        public COMMAND_PRIORITY Priority
        {
            set 
            {
                if (_state == COMMAND_STATE.STATE_NONE)
                {
                    _priority = value;
                }
            }
            get 
            { 
                return _priority; 
            }
        }

        /// <summary>
        /// Command State
        /// </summary>
        private COMMAND_STATE _state;
        public COMMAND_STATE State
        {
            get 
            { 
                return _state; 
            }
            set
            {
                _state = value;
            }
        }

        /// <summary>
        /// Callback used by dispatcher
        /// </summary>
        private Dispatcher _dispatcher;
        public Dispatcher CallBackDispather
        {
            set 
            {
                if (_state == COMMAND_STATE.STATE_NONE)
                {
                    _dispatcher = value;
                }
            }
            get 
            { 
                return _dispatcher; 
            }
        }

        private bool _isUseDispatcher = true;
        public bool IsUseDispatcher
        {
            set
            {
                _isUseDispatcher = value;
            }
            get
            {
                return _isUseDispatcher;
            }
        }

        private void Clear()
        {
            _executed = null;
            _canExecuted = null;
            _complete = null;
            _parameter = null;
            _dispatcher = null;
        }

        public ThreadCommand()
        {
            Clear();
            _priority = COMMAND_PRIORITY.PRIORITY_NORMAL;
            _state = COMMAND_STATE.STATE_NONE;
        }

        public ThreadCommand(string commandID,ExecutedThreadEventHandle executed)
        {
            Clear();
            _executed += executed;
            _commandID = commandID;
            _priority = COMMAND_PRIORITY.PRIORITY_NORMAL;
            _state = COMMAND_STATE.STATE_NONE;
        }

        ~ThreadCommand()
        {
            Dispose();
        }

        #region IDisposable Members

        public void Dispose()
        {
            Clear();
        }

        virtual protected void Dispose(bool flg)
        {
        }

        #endregion
    }
}
