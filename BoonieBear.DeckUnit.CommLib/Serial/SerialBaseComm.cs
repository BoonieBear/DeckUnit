using System;
using System.IO.Ports;
using System.Linq;

namespace BoonieBear.DeckUnit.CommLib.Serial
{
    

    /// <summary>
    /// 串口打包协议基类
    /// </summary>
    public abstract class SerialBaseComm : IObserver<CustomEventArgs>
    {
        public SerialPort _serialPort;
        protected string _str;
        protected Byte[] _nBytes;
      

        public virtual void GetMsg(string str)
        {
            _str = str;
        }

        public virtual void LoadData(Byte[] bytes)
        {
            _nBytes = new byte[bytes.Length];
            Array.Copy(bytes, _nBytes, bytes.Length);
        }

        public void SetEndChars(string endchar)
        {
            _serialPort.NewLine = endchar;
        } 
        public virtual bool SendData(out string err)
        {
            try
            {
                 if (!_serialPort.IsOpen)
                {
                    err = "串口未打开";
                    return false;
                }
                _serialPort.Write(_nBytes, 0, _nBytes.Count());
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
                if (!_serialPort.IsOpen)
                {
                    err = "串口未打开";
                    return false;
                }
                var data = _str.ToCharArray();
                _serialPort.Write(data, 0, data.Length);
                err = "发送成功";
                return true;
            }
            catch (Exception e)
            {
                err = e.Message;
                return false;
            }
            
        }


        public abstract bool Send(out string error);

        public virtual CustomEventArgs RecvData()
        {
            return null;
        }

        public virtual void Handle(object sender, CustomEventArgs e)
        { }
    }

}
