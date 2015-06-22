using System;
using System.Net.Sockets;
using System.Text;

namespace BoonieBear.DeckUnit.CommLib.TCP
{
    public abstract class TCPBaseComm:Observer<CustomEventArgs>
    {
        protected NetworkStream _TCPStream;
        protected string _str;
        protected Byte[] _nBytes;
        public abstract bool Send(out string error);

        public virtual CustomEventArgs RecvData()
        {
            return null;
        }

        public virtual bool SendData(out string err)
        {
            try
            {
                if (!_TCPStream.CanWrite)
                {
                    err = "无法发送数据";
                    return false;
                }
                _TCPStream.Write(_nBytes, 0, _nBytes.Length);
                err = "发送成功";
                return true;
            }
            catch (Exception e)
            {

                err = e.Message;
                return false;
            }
        }
        public virtual bool SendMsg(out string err)
        {
            try
            {
                if (!_TCPStream.CanWrite)
                {
                    err = "无法发送数据";
                    return false;
                }
                var data = Encoding.Default.GetBytes(_str);
                _TCPStream.Write(data, 0, data.Length);
                err = "发送成功";
                return true;
            }
            catch (Exception e)
            {
                err = e.Message;
                return false;
            }
        }

        public virtual bool Init(TcpClient tcpClient)
        {
            _TCPStream = tcpClient.GetStream();
            if (_TCPStream != null)
            {
                if(_TCPStream.CanWrite)
                    return true;
            }
            return false;
            
        }

        public virtual void GetMsg(string str)
        {
            _str = str;
        }

        public virtual void LoadData(byte[] bytes)
        {
            _nBytes = new byte[bytes.Length];
            Array.Copy(bytes, _nBytes, bytes.Length);
        }

        public virtual void Handle(object sender, CustomEventArgs e)
        {
            
        }
    }
}
