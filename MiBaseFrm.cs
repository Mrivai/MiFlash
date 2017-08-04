

using System.Windows.Forms;
using XiaoMiFlash.code.lan;

namespace XiaoMiFlash
{
  public class MiBaseFrm : Form, ILanguageSupport
  {
    private string lanid = "";

    public string LanID
    {
      get
      {
        return this.lanid;
      }
      set
      {
        this.lanid = value;
      }
    }

    public virtual void SetLanguage()
    {
    }
  }
}
