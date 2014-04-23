using BoonieBear.TinyMetro.WPF.Controller;
using BoonieBear.TinyMetro.WPF.EventAggregation;

namespace BoonieBear.DeckUnit.Core.Controllers
{
    /// <summary>
    /// 模块间消息处理类，包括WriteLog，系统消息广播，报警等
    /// 由于不像导航消息处理类那样已经由BaseController处理了一些基本消息
    /// 因此需要自己将消息处理函数完成并完成IMessageController接口
    /// </summary>
    class UnitMessageController : IMessageController
    {
        #region 构造
        private IEventAggregator eventAggregator;
        public UnitMessageController()
        {
            eventAggregator = Kernel.Instance.EventAggregator;
            //将类实例注册到EventAggregator
            eventAggregator.Subscribe(this);
        }

         ~UnitMessageController()
        {
            eventAggregator.Unsubscribe(this);
        }
        #endregion
        #region IMessage接口实现 
         public void Init()
        {
            
        }
        
        public void SendMessage(string message)
        {
            throw new System.NotImplementedException();
        }

        public void WriteLog(string message)
        {
            throw new System.NotImplementedException();
        }

        public void Alert(string message)
        {
            throw new System.NotImplementedException();
        }

        public void BroadCast(string message)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
