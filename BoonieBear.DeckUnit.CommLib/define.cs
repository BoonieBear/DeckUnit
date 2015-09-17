namespace BoonieBear.DeckUnit.CommLib
{
    public enum SerialServiceMode
    {
        LoaderMode = 0,
        HexMode = 1,
    }
    /// <summary>
    /// 调用模式：NoneMode,无模式：用于UDP调试接收，TCPshell接收
    /// AnsMode，应答模式：用于串口命令应答接收，TCP数据应答接收
    /// DataMode，数据模式：用于串口转发数据，TCP数据接收，UDP数据接收
    /// CommData，串口数据，用于接收特殊数据，状态，报警等
    /// LoaderMode，loader模式：用于串口loader模式通信
    /// ErrMode:接收过程发生错误。
    /// </summary>
    public enum CallMode
    {
        NoneMode =0,
        AnsMode = 1,
        DataMode =2,
        CommData=3,
        LoaderMode =4,
        ErrMode = 5,
        /// 4500中出现的调用模式，都是从UDP接收而来
        GPS=6,
        USBL=7,
        Sail=8,//潜器中udp接收到的数据
    }

    public enum ACNCommandMode
    {
        //shell string/loader
        CmdCharMode=0,
        //cmd,data
        CmdWithData = 1,

        
    }

   
}
