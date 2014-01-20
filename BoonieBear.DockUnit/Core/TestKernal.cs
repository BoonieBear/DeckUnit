using BoonieBear.TinyMetro.WPF.Controller;
using BoonieBear.TinyMetro.WPF.EventAggregation;

namespace BoonieBear.DockUnit.Core
{
    internal class TestKernal : Kernel
    {
        private INavigationController _controller;
        private EventAggregator _eventAggregator;

        #region Overrides of Kernel

        /// <summary>
        ///     Gets the event aggregator. It is used to send messages (e.g. in order to navigate between views)
        /// </summary>
        public override IEventAggregator EventAggregator
        {
            get { return _eventAggregator ?? (_eventAggregator = new EventAggregator()); }
        }

        /// <summary>
        ///     Gets the concrete Controller. It is used to manage the navigation flow
        /// </summary>
        public override INavigationController Controller
        {
            get { return _controller ?? (_controller = new TestController()); }
        }

        #endregion
    }
}