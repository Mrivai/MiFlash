
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using XiaoMiFlash.code.bl;
using XiaoMiFlash.code.data;
using XiaoMiFlash.code.lan;
using XiaoMiFlash.code.MiControl;
using XiaoMiFlash.code.module;
using XiaoMiFlash.code.Utility;
using XiaoMiFlash.form;

namespace XiaoMiFlash
{
    public class MainFrm : MiBaseFrm
    {
        private ProcessFrm frm = new ProcessFrm();
        private IContainer components;
        private TextBox txtPath;
        private Button btnBrwDic;
        private FolderBrowserDialog fbdSelect;
        private Button btnRefresh;
        private Button btnFlash;
        private ListView devicelist;
        private ColumnHeader clnID;
        private ColumnHeader clnDevice;
        private ColumnHeader clnProgress;
        private ColumnHeader clnTime;
        private ColumnHeader clnStatus;
        private ColumnHeader clnResult;
        private RichTextBox txtLog;
        private System.Windows.Forms.Timer timer_updateStatus;
        private StatusStrip statusStrp;
        private ToolStripStatusLabel statusLblMsg;
        private ToolStripStatusLabel statusTab;
        private RadioStripItem rdoCleanAll;
        private RadioStripItem rdoSaveUserData;
        private RadioStripItem rdoCleanAllAndLock;
        private Label lblMD5;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem miConfiguration;
        private ToolStripMenuItem miFlashConfigurationToolStripMenuItem;

        public MainFrm()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();
            txtPath = new TextBox();
            btnBrwDic = new Button();
            fbdSelect = new FolderBrowserDialog();
            btnRefresh = new Button();
            btnFlash = new Button();
            devicelist = new ListView();
            clnID = new ColumnHeader();
            clnDevice = new ColumnHeader();
            clnProgress = new ColumnHeader();
            clnTime = new ColumnHeader();
            clnStatus = new ColumnHeader();
            clnResult = new ColumnHeader();
            txtLog = new RichTextBox();
            timer_updateStatus = new System.Windows.Forms.Timer(components);
            statusStrp = new StatusStrip();
            statusLblMsg = new ToolStripStatusLabel();
            statusTab = new ToolStripStatusLabel();
            rdoCleanAll = new RadioStripItem();
            rdoSaveUserData = new RadioStripItem();
            rdoCleanAllAndLock = new RadioStripItem();
            lblMD5 = new Label();
            menuStrip1 = new MenuStrip();
            miConfiguration = new ToolStripMenuItem();
            miFlashConfigurationToolStripMenuItem = new ToolStripMenuItem();
            statusStrp.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // txtPath
            // 
            txtPath.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
            | AnchorStyles.Left)
            | AnchorStyles.Right);
            txtPath.Location = new Point(98, 33);
            txtPath.Name = "txtPath";
            txtPath.Size = new Size(668, 20);
            txtPath.TabIndex = 0;
            // 
            // btnBrwDic
            // 
            btnBrwDic.Location = new Point(21, 31);
            btnBrwDic.Name = "btnBrwDic";
            btnBrwDic.Size = new Size(75, 23);
            btnBrwDic.TabIndex = 1;
            btnBrwDic.Text = "Pilih";
            btnBrwDic.UseVisualStyleBackColor = true;
            btnBrwDic.Click += new EventHandler(btnBrwDic_Click);
            // 
            // fbdSelect
            // 
            fbdSelect.Description = "Pilih Folder SW";
            // 
            // btnRefresh
            // 
            btnRefresh.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            btnRefresh.Location = new Point(831, 29);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(75, 23);
            btnRefresh.TabIndex = 2;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += new EventHandler(btnRefresh_Click);
            // 
            // btnFlash
            // 
            btnFlash.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            btnFlash.Location = new Point(951, 28);
            btnFlash.Name = "btnFlash";
            btnFlash.Size = new Size(75, 23);
            btnFlash.TabIndex = 3;
            btnFlash.Text = "flash";
            btnFlash.UseVisualStyleBackColor = true;
            btnFlash.Click += new EventHandler(btnFlash_Click);
            // 
            // devicelist
            // 
            devicelist.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom
            | AnchorStyles.Left
            | AnchorStyles.Right);
            devicelist.Columns.AddRange(new ColumnHeader[] {
            clnID,
            clnDevice,
            clnProgress,
            clnTime,
            clnStatus,
            clnResult});
            devicelist.GridLines = true;
            devicelist.Location = new Point(21, 86);
            devicelist.Name = "devicelist";
            devicelist.Size = new Size(1005, 316);
            devicelist.TabIndex = 4;
            devicelist.UseCompatibleStateImageBehavior = false;
            devicelist.View = View.Details;
            devicelist.ColumnWidthChanging += new ColumnWidthChangingEventHandler(devicelist_ColumnWidthChanging);
            // 
            // clnID
            // 
            clnID.Text = "ID";
            // 
            // clnDevice
            // 
            clnDevice.Text = "Kenzo";
            clnDevice.Width = 90;
            // 
            // clnProgress
            // 
            clnProgress.Text = "Progres";
            clnProgress.Width = 107;
            // 
            // clnTime
            // 
            clnTime.Text = "Waktu";
            // 
            // clnStatus
            // 
            clnStatus.Text = "Status";
            clnStatus.Width = 500;
            // 
            // clnResult
            // 
            clnResult.Text = "Hasil";
            clnResult.Width = 126;
            // 
            // txtLog
            // 
            txtLog.Location = new Point(37, 421);
            txtLog.Name = "txtLog";
            txtLog.Size = new Size(949, 65);
            txtLog.TabIndex = 6;
            txtLog.Text = "";
            txtLog.Visible = false;
            // 
            // timer_updateStatus
            // 
            timer_updateStatus.Interval = 1000;
            timer_updateStatus.Tick += new EventHandler(timer_updateStatus_Tick);
            // 
            // statusStrp
            // 
            statusStrp.Items.AddRange(new ToolStripItem[] {
            statusLblMsg,
            statusTab,
            rdoCleanAll,
            rdoSaveUserData,
            rdoCleanAllAndLock});
            statusStrp.Location = new Point(0, 422);
            statusStrp.Name = "statusStrp";
            statusStrp.Size = new Size(1094, 22);
            statusStrp.TabIndex = 7;
            statusStrp.Text = "statusStrip1";
            // 
            // statusLblMsg
            // 
            statusLblMsg.Name = "statusLblMsg";
            statusLblMsg.Size = new System.Drawing.Size(0, 17);
            // 
            // statusTab
            // 
            statusTab.Name = "statusTab";
            statusTab.Size = new System.Drawing.Size(708, 17);
            statusTab.Spring = true;
            // 
            // rdoCleanAll
            // 
            rdoCleanAll.IsChecked = false;
            rdoCleanAll.Name = "rdoCleanAll";
            rdoCleanAll.Size = new System.Drawing.Size(98, 20);
            rdoCleanAll.Text = "Hapus Semua";
            // 
            // rdoSaveUserData
            // 
            rdoSaveUserData.IsChecked = true;
            rdoSaveUserData.Name = "rdoSaveUserData";
            rdoSaveUserData.Size = new System.Drawing.Size(118, 20);
            rdoSaveUserData.Text = "Simpan Data User";
            // 
            // rdoCleanAllAndLock
            // 
            rdoCleanAllAndLock.IsChecked = false;
            rdoCleanAllAndLock.Name = "rdoCleanAllAndLock";
            rdoCleanAllAndLock.Size = new System.Drawing.Size(155, 20);
            rdoCleanAllAndLock.Text = "Hapus Semua Dan Kunci";
            // 
            // lblMD5
            // 
            lblMD5.AutoSize = true;
            lblMD5.Location = new System.Drawing.Point(96, 68);
            lblMD5.Name = "lblMD5";
            lblMD5.Size = new System.Drawing.Size(0, 13);
            lblMD5.TabIndex = 8;
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = SystemColors.ControlLight;
            menuStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            miConfiguration});
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            menuStrip1.Size = new System.Drawing.Size(1094, 24);
            menuStrip1.TabIndex = 9;
            menuStrip1.Text = "menuStrip1";
            // 
            // miConfiguration
            // 
            miConfiguration.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            miFlashConfigurationToolStripMenuItem});
            miConfiguration.Name = "miConfiguration";
            miConfiguration.Size = new System.Drawing.Size(93, 20);
            miConfiguration.Text = "Configuration";
            // 
            // miFlashConfigurationToolStripMenuItem
            // 
            miFlashConfigurationToolStripMenuItem.Name = "miFlashConfigurationToolStripMenuItem";
            miFlashConfigurationToolStripMenuItem.Size = new Size(178, 22);
            miFlashConfigurationToolStripMenuItem.Text = "Konfigurasi MiFlash";
            miFlashConfigurationToolStripMenuItem.Click += new EventHandler(miFlashConfigurationToolStripMenuItem_Click);
            // 
            // MainFrm
            // 
            ClientSize = new Size(1094, 444);
            Controls.Add(lblMD5);
            Controls.Add(statusStrp);
            Controls.Add(menuStrip1);
            Controls.Add(txtLog);
            Controls.Add(devicelist);
            Controls.Add(btnFlash);
            Controls.Add(btnRefresh);
            Controls.Add(btnBrwDic);
            Controls.Add(txtPath);
            MainMenuStrip = menuStrip1;
            Name = "MainFrm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "XiaoMiFlash(Beta)";
            FormClosing += new FormClosingEventHandler(MainFrm_FormClosing);
            FormClosed += new FormClosedEventHandler(MainFrm_FormClosed);
            Load += new EventHandler(MainFrm_Load);
            statusStrp.ResumeLayout(false);
            statusStrp.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        private bool IsRunAsAdmin()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            SetLanguage();
            txtPath.Text = MiAppConfig.Get("swPath");
        }



        private void btnBrwDic_Click(object sender, EventArgs e)
        {
            if (fbdSelect.ShowDialog() != DialogResult.OK)
                return;
            txtPath.Text = fbdSelect.SelectedPath;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                btnRefresh.Enabled = false;
                btnRefresh.Cursor = Cursors.WaitCursor;
                List<Device> device = UsbDevice.GetDevice();
                ListView.ListViewItemCollection items = devicelist.Items;
                foreach (string str in FlashingDevice.flashDeviceList.Where(d => d.IsDone.Value).Select(d => d.Name).ToList())
                {
                    foreach (Device flashDevice in FlashingDevice.flashDeviceList)
                    {
                        if (flashDevice.Name == str.ToString())
                        {
                            FlashingDevice.flashDeviceList.Remove(flashDevice);
                            break;
                        }
                    }
                    foreach (ListViewItem listViewItem in items)
                    {
                        if (listViewItem.SubItems[1].Text == str.ToString())
                        {
                            items.Remove(listViewItem);
                            break;
                        }
                    }
                    foreach (Control control in (ArrangedElementCollection)devicelist.Controls)
                    {
                        if (control.Name == str.ToString() + "progressbar")
                        {
                            devicelist.Controls.Remove(control);
                            break;
                        }
                    }
                }
                using (List<Device>.Enumerator enumerator = device.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Device d = enumerator.Current;
                        if (FlashingDevice.flashDeviceList.Where(fd => fd.Name == d.Name).Select(fd => fd.Name).Count() == 0)
                        {
                            int num1 = devicelist.Items.Count + 1;
                            ListViewItem listViewItem = new ListViewItem(new string[6]
                            {
                num1.ToString(),
                d.Name,
                "",
                "0s",
                "",
                ""
                            });
                            devicelist.Items.Add(listViewItem);
                            d.ID = num1;
                            d.Progress = 0.0f;
                            d.IsDone = new bool?();
                            FlashingDevice.flashDeviceList.Add(d);
                            float num2 = 0.0f;
                            Rectangle rectangle = new Rectangle();
                            ProgressBar progressBar = new ProgressBar();
                            rectangle = listViewItem.SubItems[2].Bounds;
                            rectangle.Width = devicelist.Columns[2].Width;
                            progressBar.Parent = devicelist;
                            progressBar.SetBounds(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
                            progressBar.Value = (int)num2;
                            progressBar.Visible = true;
                            progressBar.Name = d.Name + "progressbar";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.w(ex.Message);
                int num = (int)MessageBox.Show(ex.Message);
            }
            finally
            {
                btnRefresh.Enabled = true;
                btnRefresh.Cursor = Cursors.Default;
            }
        }

        private void btnFlash_Click(object sender, EventArgs e)
        {
            string str = "";
            if (rdoCleanAll.IsChecked)
                str = FlashType.CleanAll;
            else if (rdoSaveUserData.IsChecked)
                str = FlashType.SaveUserData;
            else if (rdoCleanAllAndLock.IsChecked)
                str = FlashType.CleanAllAndLock;
            timer_updateStatus.Enabled = true;
            try
            {
                foreach (Device flashDevice in FlashingDevice.flashDeviceList)
                {
                    if (!flashDevice.IsDone.HasValue || flashDevice.IsDone.Value)
                    {
                        flashDevice.StartTime = DateTime.Now;
                        flashDevice.Status = "flashing";
                        flashDevice.Progress = 0.0f;
                        flashDevice.IsDone = new bool?(false);
                        flashDevice.IsUpdate = true;
                        DeviceCtrl deviceCtrl = flashDevice.DeviceCtrl;
                        deviceCtrl.deviceName = flashDevice.Name;
                        deviceCtrl.swPath = txtPath.Text.Trim();
                        deviceCtrl.flashScript = str;
                        new Thread(new ThreadStart(deviceCtrl.flash))
                        {
                            IsBackground = true
                        }.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show(ex.Message);
            }
        }

        private void timer_updateStatus_Tick(object sender, EventArgs e)
        {
            foreach (ListViewItem listViewItem in devicelist.Items)
            {
                listViewItem.UseItemStyleForSubItems = false;
                foreach (Device flashDevice in FlashingDevice.flashDeviceList)
                {
                    if (flashDevice.IsUpdate && flashDevice.Name.ToLower() == listViewItem.SubItems[1].Text.ToLower())
                    {
                        listViewItem.SubItems[2].Text = (flashDevice.Progress * 100.0).ToString() + "%";
                        foreach (Control control in (ArrangedElementCollection)devicelist.Controls)
                        {
                            if (control.Name == flashDevice.Name + "progressbar")
                            {
                                ProgressBar progressBar = (ProgressBar)control;
                                if (progressBar.Value == (int)(flashDevice.Progress * 100.0))
                                {
                                    if ((int)(flashDevice.Progress * 100.0) < 100)
                                        flashDevice.Progress += 3f / 1000f;
                                    progressBar.Value = (int)(flashDevice.Progress * 100.0);
                                }
                                else
                                    progressBar.Value = (int)(flashDevice.Progress * 100.0);
                            }
                        }
                        if (flashDevice.StartTime > DateTime.MinValue)
                        {
                            TimeSpan timeSpan = DateTime.Now.Subtract(flashDevice.StartTime);
                            listViewItem.SubItems[3].Text = string.Format("{0}s", timeSpan.TotalSeconds.ToString());
                        }
                        listViewItem.SubItems[4].Text = flashDevice.Status;
                        listViewItem.SubItems[5].Text = flashDevice.Result;
                        if (flashDevice.IsDone.HasValue && flashDevice.IsDone.Value || flashDevice.Status == "flash done")
                        {
                            flashDevice.IsUpdate = false;
                            flashDevice.IsDone = new bool?(true);
                            listViewItem.SubItems[5].BackColor = Color.LightGreen;
                        }
                        if (flashDevice.IsDone.HasValue && flashDevice.IsDone.Value || flashDevice.Result.ToLower() == "success")
                        {
                            flashDevice.IsUpdate = false;
                            flashDevice.IsDone = new bool?(true);
                            listViewItem.SubItems[5].BackColor = Color.LightGreen;
                            break;
                        }
                        if (flashDevice.Result.ToLower().IndexOf("error") < 0)
                        {
                            if (flashDevice.Result.ToLower().IndexOf("fail") < 0)
                                break;
                        }
                        flashDevice.IsUpdate = false;
                        flashDevice.IsDone = new bool?(true);
                        listViewItem.SubItems[5].BackColor = Color.Red;
                        break;
                    }
                }
            }
            if (FlashingDevice.flashDeviceList.Count != 0)
                return;
            timer_updateStatus.Enabled = false;
        }

        private void devicelist_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            int newWidth = e.NewWidth;
            foreach (Control control in (ArrangedElementCollection)devicelist.Controls)
            {
                if (control.Name.IndexOf("progressbar") >= 0)
                {
                    ProgressBar progressBar = (ProgressBar)control;
                    Rectangle bounds = progressBar.Bounds;
                    bounds.Width = devicelist.Columns[2].Width;
                    progressBar.SetBounds(devicelist.Items[0].SubItems[2].Bounds.X, bounds.Y, bounds.Width, bounds.Height);
                }
            }
        }

        private void MainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MiAppConfig.SetValue("swPath", txtPath.Text.ToString());
        }

        private void MainFrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
            Dispose();
            Close();
        }

        public override void SetLanguage()
        {
            base.SetLanguage();
            if (CultureInfo.InstalledUICulture.Name.ToLower().IndexOf("zh") >= 0)
                LanID = LanguageType.eng;
            else
                LanID = LanguageType.eng;
            LanguageProvider languageProvider = new LanguageProvider(LanID);
            btnBrwDic.Text = languageProvider.GetLanguage("MainFrm.btnBrwDic");
            btnRefresh.Text = languageProvider.GetLanguage("MainFrm.btnRefresh");
            btnFlash.Text = languageProvider.GetLanguage("MainFrm.btnFlash");
            devicelist.Columns[0].Text = languageProvider.GetLanguage("MainFrm.devicelist.cln0");
            devicelist.Columns[1].Text = languageProvider.GetLanguage("MainFrm.devicelist.cln1");
            devicelist.Columns[2].Text = languageProvider.GetLanguage("MainFrm.devicelist.cln2");
            devicelist.Columns[3].Text = languageProvider.GetLanguage("MainFrm.devicelist.cln3");
            devicelist.Columns[4].Text = languageProvider.GetLanguage("MainFrm.devicelist.cln4");
            devicelist.Columns[5].Text = languageProvider.GetLanguage("MainFrm.devicelist.cln5");
            rdoCleanAll.Text = languageProvider.GetLanguage("MainFrm.rdoCleanAll");
            rdoSaveUserData.Text = languageProvider.GetLanguage("MainFrm.rdoSaveUserData");
            rdoCleanAllAndLock.Text = languageProvider.GetLanguage("MainFrm.rdoCleanAllAndLock");
        }

        private void miFlashConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ConfigurationFrm().Show();
        }
    }
}
