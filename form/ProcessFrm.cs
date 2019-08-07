

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace XiaoMiFlash.form
{
    public class ProcessFrm : Form
    {
        private Label label1;
        private PictureBox pictureBox1;

        public ProcessFrm()
        {
            InitializeComponent();
        }

        public void showMD5(Hashtable table)
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ProcessFrm));
            label1 = new Label();
            pictureBox1 = new PictureBox();
            ((ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            label1.AutoSize = true;
            label1.Location = new Point(91, 29);
            label1.Name = "label1";
            label1.Size = new Size(89, 12);
            label1.TabIndex = 0;
            label1.Text = "MD5 validation";
            pictureBox1.Image = (Image)componentResourceManager.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(103, 92);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(100, 50);
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            AutoScaleDimensions = new SizeF(6f, 12f);
            AutoSize = true;
            ClientSize = new Size(284, 262);
            Controls.Add(pictureBox1);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "processFrm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "processFrm";
            TopMost = true;
            ((ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
