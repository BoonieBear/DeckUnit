using BoonieBear.DeckUnit.Core.Controllers;
using BoonieBear.DeckUnit.ICore;
using BoonieBear.TinyMetro.WPF.Controller;
using BoonieBear.TinyMetro.WPF.EventAggregation;

namespace BoonieBear.DeckUnit.Core
{
    internal class UnitKernal : IUnitKernel
    {
        public static IUnitKernel Instance { get; set; }
        private INavigationController _navicontroller;
        private EventAggregator _eventAggregator;
        private IMessageController _messageController;
        #region Overrides of Kernel

        /// <summary>
        ///     Gets the event aggregator. It is used to send messages (e.g. in order to navigate between views)
        /// </summary>
        public  IEventAggregator EventAggregator
        {
            get { return _eventAggregator ?? (_eventAggregator = new EventAggregator()); }
        }

        /// <summary>
        ///     导航命令
        /// </summary>
        public INavigationController Controller
        {
            get { return _navicontroller ?? (_navicontroller = new UnitNavigationController()); }
        }
        /// <summary>
        /// 消息
        /// </summary>
        public IMessageController MessageController
        {
            get { return _messageController ?? (_messageController = new UnitMessageController()); }
            
        }

        #endregion
    }
}