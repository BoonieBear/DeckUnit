using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SUSI.Library;
using System.Diagnostics;

namespace BoonieBear.DeckUnit.Comm.GPIO
{
    public class GPIOService
    {
        UInt32 InCount;
        UInt32 OutCount;
        UInt32 targetPinMask;
        public UInt32 StatusMask;

        public GPIOService()
        {

        }

        public void Initialize_SUSI()
        {
            try
            {
                if (!SUSI_DLL.SusiDllInit())
                {
                    throw new Exception("SusiDllInit failed! " + "ErrorCode = " + SUSI_DLL.SusiDllGetLastError().ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            Init_Page_GPIOEX();//初始化GPIO
            GetPinNum();
        }

        private void Init_Page_GPIOEX()
        {
            try
            {

                int ret = SUSI_GPIO.SusiIOAvailable();
                if (ret < 0)
                    throw new Exception("SusiIOAvailable failed! " + "ErrorCode = " + SUSI_DLL.SusiDllGetLastError().ToString());
                else if (ret == 1)
                {
                    // tabPageGPIOEx.Controls.Add(new PageGPIOEX());
                }
                else
                {
                    //tabControl.Controls.Remove(tabPageGPIOEx);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void GetPinNum()
        {
            try
            {
                unsafe
                {
                    UInt32 inCount, outCount;
                    if (!SUSI_GPIO.SusiIOCountEx(&inCount, &outCount))
                    {
                        throw new Exception("SusiIOCountEx failed! " + "ErrorCode = " + SUSI_DLL.SusiDllGetLastError().ToString());
                    }
                    InCount = inCount;
                    OutCount = outCount;

                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            

        }

        public void IORead()
        {
            try
            {
                unsafe
                {
                    {
                        targetPinMask = (UInt32)Math.Pow(2, InCount + OutCount) - 1;
                        UInt32 statusMask;
                        if (!SUSI_GPIO.SusiIOReadMultiEx(targetPinMask, &statusMask))
                        {
                            throw new Exception("SusiIOReadMultiEx failed! " + "ErrorCode = " + SUSI_DLL.SusiDllGetLastError().ToString());
                        }
                        StatusMask = statusMask;
                    }

                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
