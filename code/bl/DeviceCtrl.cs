

namespace XiaoMiFlash.code.bl
{
  public abstract class DeviceCtrl
  {
    public string swPath = "D:\\SW\\A1\\FDL153I\\images\\";
    public string deviceName = "";
    public string flashScript;

    public abstract void flash();

    public abstract string[] getDevice();
  }
}
