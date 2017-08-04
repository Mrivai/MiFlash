

using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace XiaoMiFlash.code.MiControl
{
  [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip | ToolStripItemDesignerAvailability.ContextMenuStrip | ToolStripItemDesignerAvailability.StatusStrip)]
  public class RadioStripItem : ToolStripControlHost
  {
    private RadioButton radio;

    public bool IsChecked
    {
      get
      {
        return radio.Checked;
      }
      set
      {
        radio.Checked = value;
      }
    }

    public RadioStripItem()
      : base(new RadioButton())
    {
      radio = Control as RadioButton;
    }
  }
}
