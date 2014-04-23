using System;

namespace BoonieBear.DeckUnit.Core.Controllers
{
    public interface IMessageController
    {
        void Init();
        //发送信息到系统记录
        void SendMessage(string message);
        //将log写入文件
        void WriteLog(string message);
        //发送消息到界面
        void Alert(string message);
        //通过UDP调试端口广播信息，用于调试
        void BroadCast(string message);
    }
}
