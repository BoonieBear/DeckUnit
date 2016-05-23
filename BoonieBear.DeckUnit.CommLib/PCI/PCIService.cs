using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Automation.BDaq;
using System.Diagnostics;

namespace BoonieBear.DeckUnit.Comm.PCI
{
    public class PCIService
    {
        private Automation.BDaq.InstantDiCtrl instantDiCtrl1=new InstantDiCtrl();
        private const int m_startPort = 0;
        public  const int m_portCountShow = 4;
        public byte[] portData = new byte[m_portCountShow];
        private string str = "PCI-1730,BID#0";
        private int DeviceCount = 0;

        public PCIService()
        {

        }

        public bool Initialize_PCI()
        {
            try
            {
                DeviceCount = instantDiCtrl1.SupportedDevices.Count;
                for (int i = 0; i < DeviceCount; i++)
                {
                    instantDiCtrl1.SelectedDevice = new DeviceInformation(i);
                    if (instantDiCtrl1.SelectedDevice.Description == str)
                    {
                        break;
                    }
                    if (i == DeviceCount - 1)
                    {
                        return false;
                    }

                }
                if (!instantDiCtrl1.Initialized)
                {
                    return false;
                }
                else 
                {
                    return true;
                }

            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public void IORead()
        {
            try
            {
                // read Di port state
                ErrorCode err = ErrorCode.Success;

                for (int i = 0; (i + m_startPort) < instantDiCtrl1.Features.PortCount && i < m_portCountShow; ++i)
                {
                    err = instantDiCtrl1.Read(i + m_startPort, out portData[i]);
                    if (err != ErrorCode.Success)
                    {
                        throw new Exception("Sorry ! Some errors happened, the error code is: "+err.ToString());
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
