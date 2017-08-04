

using Microsoft.Win32;
using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using XiaoMiFlash.code.Utility;

namespace XiaoMiFlash
{
    [RunInstaller(true)]
    public class MiInstaller : Installer
    {
        private IContainer components;

        public MiInstaller()
        {
            InitializeComponent();
            BeforeInstall += new InstallEventHandler(MiInstaller_BeforeInstall);
            AfterInstall += new InstallEventHandler(MiInstaller_AfterInstall);
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
        }

        private void MiInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            CopyFiles();
            InstallAllDriver();
        }

        public override void Install(IDictionary savedState)
        {
            try
            {
                base.Install(savedState);
            }
            catch (Exception)
            {
            }
        }

        private void MiInstaller_BeforeInstall(object sender, InstallEventArgs e)
        {
        }

        private void CopyFiles()
        {
            string parameter = Context.Parameters["assemblypath"];
            string installationPath = parameter.Substring(0, parameter.LastIndexOf('\\') + 1);
            try
            {
                string systemDirectory = Environment.SystemDirectory;
                string[] strArray1 = new string[1]
                {
                    "Qualcomm\\Driver\\serial\\i386\\qcCoInstaller.dll"
                };
                string[] strArray2 = new string[1]
                {
                    systemDirectory + "\\qcCoInstaller.dll"
                };
                string str = installationPath + "Source\\ThirdParty\\";
                for (int index = 0; index < strArray1.Length; ++index)
                {
                    File.Copy(str + strArray1[index], strArray2[index], false);
                    Log.Installw(installationPath, string.Format("copy {0} to {1}", str + strArray1[index], strArray2[index]));
                }
            }
            catch (Exception ex)
            {
                Log.Installw(installationPath, string.Format("copy file failed,{0}", ex.Message));
            }
        }

        private void InstallAllDriver()
        {
            string parameter = Context.Parameters["assemblypath"];
            string installationPath = parameter.Substring(0, parameter.LastIndexOf('\\') + 1);
            string[] strArray = new string[5]
            {
                "Google\\Driver\\android_winusb.inf",
                "Nvidia\\Driver\\NvidiaUsb.inf",
                "Microsoft\\Driver\\tetherxp.inf",
                "Microsoft\\Driver\\wpdmtphw.inf",
                "Qualcomm\\Driver\\qcser.inf"
            };
            string path = installationPath + "Source\\ThirdParty\\";
            if (new DirectoryInfo(path).Exists)
            {
                for (int index = 0; index < strArray.Length; ++index)
                    InstallDriver(path + strArray[index], installationPath);
            }
            else
                Log.Installw(installationPath, "dic " + path + " not exists.");
        }

        private void InstallDriver(string infPath, string installationPath)
        {
            try
            {
                string str1 = "Software\\XiaoMi\\MiFlash\\";
                FileInfo fileInfo = new FileInfo(infPath);
                RegistryKey localMachine = Registry.LocalMachine;
                RegistryKey registryKey = localMachine.OpenSubKey(str1, true);
                Log.Installw(installationPath, string.Format("open RegistryKey {0}", str1));
                if (registryKey == null)
                {
                    registryKey = localMachine.CreateSubKey(str1, RegistryKeyPermissionCheck.ReadWriteSubTree);
                    Log.Installw(installationPath, string.Format("create RegistryKey {0}", str1));
                }
                registryKey.GetValueNames();
                registryKey.GetValue(fileInfo.Name);
                bool success = true;
                string destinationInfFileNameComponent = "";
                string destinationInfFileName = "";
                string str2 = Driver.SetupOEMInf(fileInfo.FullName, out destinationInfFileName, out destinationInfFileNameComponent, out success);
                Log.Installw(installationPath, string.Format("install driver {0} to {1},result {2},GetLastWin32Error {3}", fileInfo.FullName, destinationInfFileName, success.ToString(), str2));
                if (success)
                {
                    registryKey.SetValue(fileInfo.Name, destinationInfFileNameComponent);
                    Log.Installw(installationPath, string.Format("set RegistryKey value:{0}--{1}", fileInfo.Name, destinationInfFileNameComponent));
                }
                registryKey.Close();
                if (infPath.IndexOf("android_winusb.inf") < 0)
                    return;
                string environmentVariable = Environment.GetEnvironmentVariable("USERPROFILE");
                Cmd cmd = new Cmd("");
                string str3 = string.Format("mkdir \"{0}\\.android\"", environmentVariable);
                string str4 = cmd.Execute(null, str3);
                Log.Installw(installationPath, str3);
                Log.Installw(installationPath, "output:" + str4);
                string str5 = string.Format(" echo 0x2717 >>\"{0}\\.android\\adb_usb.ini\"", environmentVariable);
                string str6 = cmd.Execute(null, str5);
                Log.Installw(installationPath, str5);
                Log.Installw(installationPath, "output:" + str6);
            }
            catch (Exception ex)
            {
                Log.Installw(installationPath, string.Format("install driver {0}, exception:{1}", infPath, ex.Message));
            }
        }
    }
}
