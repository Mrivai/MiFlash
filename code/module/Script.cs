
using System;

namespace XiaoMiFlash.code.module
{
  public class Script
  {
    public static string AndroidPath
    {
      get
      {
        return AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Source\\ThirdParty\\Google\\Android";
      }
    }

    public static string fastboot
    {
      get
      {
        return AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Source\\ThirdParty\\Google\\Android\\fastboot.exe";
      }
    }

    public static string QcLsUsb
    {
      get
      {
        return AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Source\\ThirdParty\\Qualcomm\\fh_loader\\lsusb.exe";
      }
    }

    public static string emmcdl
    {
        get
        {
            return AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Source\\ThirdParty\\Qualcomm\\emmcdl.exe";
        }
    }
  }
}
