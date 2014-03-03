using System;
using System.IO.Ports;
using System.Linq;

namespace BoonieBear.DeckUnit.CommLib
{
    /// <summary>
    /// 串口打包协议基类
    /// </summary>
    public abstract class SerialBaseComm : ISerialComm
    {
        private SerialPort _serialPort;
        private string _str;
        private Byte[] _nBytes;

        public virtual void GetMsg(string str)
        {
            _str = str;
        }

        public virtual void GetData(Byte[] bytes)
        {
            _nBytes = new byte[bytes.Length];
            Array.Copy(bytes, _nBytes, bytes.Length);
        }

        public void SetEndChars(string endchar)
        {
            _serialPort.NewLine = endchar;
        }
        public  bool Init(SerialPort serialPort)
        {
            _serialPort = serialPort;

            if (_serialPort.IsOpen)
            {
                return true;
            }
            return false;
        }

        public virtual void SendData()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Write(_nBytes, 0, _nBytes.Count());
            }
        }
        public virtual void SendMsg()
        {
            if (!_serialPort.IsOpen)
            {
                _serialPort.WriteLine(_str);
            }
        }
    }

}
