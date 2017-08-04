

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml;
using XiaoMiFlash.code.data;
using XiaoMiFlash.code.module;
using XiaoMiFlash.code.Utility;

namespace XiaoMiFlash.code.bl
{
    public class SerialPortDevice : DeviceCtrl
    {
        public Comm comm = new Comm();
        private int BUFFER_SECTORS = 256;
        private int programmerType = 1;
        private string storageType = "ufs";

        public override void flash()
        {
            if (string.IsNullOrEmpty(deviceName))
                return;
            if (!Directory.Exists(swPath))
                throw new Exception("sw path is not valid");
            foreach (DirectoryInfo directory in new DirectoryInfo(swPath).GetDirectories())
            {
                if (directory.Name.ToLower() == "images")
                {
                    swPath = directory.FullName;
                    break;
                }
            }
            try
            {
                FlashingDevice.UpdateDeviceStatus(deviceName, new float?(0.0f), "start flash", "flashing", false);
                registerPort(deviceName);
                SaharaDownloadProgrammer();
                Thread.Sleep(1000);
                PropareFirehose();
                ConfigureDDR(comm.intSectorSize, BUFFER_SECTORS, storageType, 0);
                if (Provision(swPath))
                {
                    Thread.Sleep(2000);
                    SaharaDownloadProgrammer();
                    Thread.Sleep(1000);
                    PropareFirehose();
                    ConfigureDDR(comm.intSectorSize, BUFFER_SECTORS, storageType, 0);
                }
                if (storageType == Storage.ufs)
                    SetBootPartition();
                if (comm.ignoreResponse)
                    comm.StartReading();
                FirehoseDownloadImg(swPath);
                comm.StopReading();
                FlashingDevice.UpdateDeviceStatus(deviceName, new float?(1f), "flash done", "success", true);
            }
            catch (Exception ex)
            {
                FlashingDevice.UpdateDeviceStatus(deviceName, new float?(), ex.Message, "error", true);
                Log.w(deviceName, ex, true);
            }
            finally
            {
                comm.serialPort.Close();
                comm.serialPort.Dispose();
            }
        }

        private void registerPort(string port)
        {
            if (comm.serialPort != null)
                comm.serialPort.Close();
            comm.serialPort.PortName = port;
            comm.serialPort.BaudRate = 9600;
            comm.serialPort.Parity = Parity.None;
            comm.serialPort.ReadTimeout = 100;
            comm.serialPort.WriteTimeout = 100;
            comm.Open();
        }

        private void SaharaDownloadProgrammer()
        {
            if (comm.IsOpen)
            {
                string msg1 = string.Format("[{0}]:{1}", comm.serialPort.PortName, "start flash.");
                FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, new float?(0.0f), "read hello packet", "flashing", false);
                Log.w(comm.serialPort.PortName, msg1);
                comm.getRecData();
                if (comm.recData.Length == 0)
                    comm.recData = new byte[48];
                sahara_packet saharaPacket = new sahara_packet();
                sahara_hello_packet stuct1 = (sahara_hello_packet)CommandFormat.BytesToStuct(comm.recData, typeof(sahara_hello_packet));
                stuct1.Reserved = new uint[6];
                sahara_hello_response saharaHelloResponse = new sahara_hello_response();
                saharaHelloResponse.Reserved = new uint[6];
                sahara_switch_Mode_packet switchModePacket = new sahara_switch_Mode_packet();
                sahara_readdata_packet saharaReaddataPacket = new sahara_readdata_packet();
                sahara_64b_readdata_packet obj = new sahara_64b_readdata_packet();
                sahara_end_transfer_packet endTransferPacket = new sahara_end_transfer_packet();
                sahara_done_response saharaDoneResponse = new sahara_done_response();
                int num = 20;
                while (num-- > 0 && (int)stuct1.Command != 1)
                {
                    string str1 = "cannot receive hello packet,try agian";
                    Log.w(comm.serialPort.PortName, str1);
                    FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, new float?(0.0f), str1, "flashing", false);
                    if (comm.recData.Length == 0)
                        comm.recData = new byte[48];
                    stuct1 = (sahara_hello_packet)CommandFormat.BytesToStuct(comm.recData, typeof(sahara_hello_packet));
                    Thread.Sleep(500);
                    if (num == 10)
                    {
                        string str2 = "cannot receive hello packet,try to reset";
                        Log.w(comm.serialPort.PortName, str2);
                        FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, new float?(0.0f), str2, "flashing", false);
                        saharaHelloResponse.Command = 2U;
                        saharaHelloResponse.Length = (uint)Marshal.SizeOf(saharaHelloResponse);
                        saharaHelloResponse.Version = 2U;
                        saharaHelloResponse.Version_min = 1U;
                        saharaHelloResponse.Mode = 3U;
                        comm.WritePort(CommandFormat.StructToBytes(saharaHelloResponse), 0, Marshal.SizeOf(saharaHelloResponse));
                        string str3 = "Switch mode back";
                        Log.w(comm.serialPort.PortName, str3);
                        FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, new float?(0.0f), str3, "flashing", false);
                        switchModePacket.Command = 12U;
                        switchModePacket.Length = (uint)Marshal.SizeOf(switchModePacket);
                        switchModePacket.Mode = 0U;
                        comm.WritePort(CommandFormat.StructToBytes(switchModePacket), 0, Marshal.SizeOf(switchModePacket));
                        if (comm.recData.Length == 0)
                            comm.recData = new byte[48];
                        stuct1 = (sahara_hello_packet)CommandFormat.BytesToStuct(comm.recData, typeof(sahara_hello_packet));
                    }
                }
                if ((int)stuct1.Command == 1)
                {
                    string str = "received hello packet";
                    FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, new float?(0.0f), str, "flashing", false);
                    Log.w(comm.serialPort.PortName, str);
                }
                else
                {
                    string str = "cannot receive hello packet";
                    FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, new float?(0.0f), str, "flashing", false);
                    Log.w(comm.serialPort.PortName, str);
                }
                saharaHelloResponse.Command = 2U;
                saharaHelloResponse.Length = 48U;
                saharaHelloResponse.Version = 2U;
                saharaHelloResponse.Version_min = 1U;
                byte[] bytes1 = CommandFormat.StructToBytes(saharaHelloResponse);
                comm.WritePort(bytes1, 0, bytes1.Length);
                string[] strArray = FileSearcher.SearchFiles(swPath, SoftwareImage.ProgrammerPattern);
                if (strArray.Length != 1)
                    throw new Exception("can not found programmer file.");
                string str4 = strArray[0];
                FileInfo fileInfo = new FileInfo(str4);
                if (fileInfo.Name.ToLower().IndexOf("firehose") >= 0)
                    programmerType = Programmer.firehose;
                if (fileInfo.Name.ToLower().IndexOf("ufs") >= 0)
                    storageType = Storage.ufs;
                else if (fileInfo.Name.ToLower().IndexOf("emmc") >= 0)
                    storageType = Storage.emmc;
                comm.intSectorSize = storageType == Storage.ufs ? comm.SECTOR_SIZE_UFS : comm.SECTOR_SIZE_EMMC;
                Log.w(comm.serialPort.PortName, "donwload programmer " + str4);
                FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, new float?(0.0f), "download programmer " + str4, "flashing", false);
                FileTransfer fileTransfer = new FileTransfer(comm.serialPort.PortName, str4);
                bool flag;
                do
                {
                    flag = false;
                    comm.getRecData();
                    byte[] recData = comm.recData;
                    saharaPacket = (sahara_packet)CommandFormat.BytesToStuct(comm.recData, typeof(sahara_packet));
                    switch (saharaPacket.Command)
                    {
                        case 3:
                            sahara_readdata_packet stuct3 = (sahara_readdata_packet)CommandFormat.BytesToStuct(comm.recData, typeof(sahara_readdata_packet));
                            string msg2 = string.Format("sahara read data:imgID {0}, offset {1},length {2}", stuct3.Image_id, stuct3.Offset, stuct3.SLength);
                            fileTransfer.transfer(comm.serialPort, (int)stuct3.Offset, (int)stuct3.SLength);
                            Log.w(comm.serialPort.PortName, msg2);
                            break;
                        case 4:
                            sahara_end_transfer_packet stuct4 = (sahara_end_transfer_packet)CommandFormat.BytesToStuct(comm.recData, typeof(sahara_end_transfer_packet));
                            string msg3 = string.Format("sahara read end  imgID:{0} status:{1}", stuct4.Image_id, stuct4.Status);
                            if ((int)stuct4.Status != 0)
                                Log.w(comm.serialPort.PortName, string.Format("sahara read end error with status:{0}", stuct4.Status));
                            flag = true;
                            Log.w(comm.serialPort.PortName, msg3);
                            break;
                        case 18:
                            sahara_64b_readdata_packet stuct5 = (sahara_64b_readdata_packet)CommandFormat.BytesToStuct(comm.recData, typeof(sahara_64b_readdata_packet));
                            string msg4 = string.Format("sahara read 64b data:imgID {0},offset {1},length {2}", stuct5.Image_id, stuct5.Offset, stuct5.SLength);
                            fileTransfer.transfer(comm.serialPort, (int)stuct5.Offset, (int)stuct5.SLength);
                            Log.w(comm.serialPort.PortName, msg4);
                            break;
                        default:
                            Log.w(comm.serialPort.PortName, string.Format("invalid command:{0}", saharaPacket.Command));
                            break;
                    }
                }
                while (!flag);
                saharaPacket.Command = 5U;
                saharaPacket.Length = 8U;
                byte[] bytes2 = CommandFormat.StructToBytes(saharaPacket, 8);
                for (int index = 8; index < bytes2.Length; ++index)
                    bytes2[index] = 0;
                comm.WritePort(bytes2, 0, bytes2.Length);
                comm.getRecData();
                if (comm.recData.Length == 0)
                    comm.recData = new byte[48];
                if ((int)((sahara_done_response)CommandFormat.BytesToStuct(comm.recData, typeof(sahara_done_response))).Command == 6)
                {
                    string str1 = string.Format("file {0} transferred successfully", str4);
                    Thread.Sleep(2000);
                    Log.w(comm.serialPort.PortName, str1);
                    FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, new float?(1f), str1, "flashing", false);
                }
                else
                {
                    string str1 = "programmer transfer error";
                    Log.w(comm.serialPort.PortName, str1);
                    FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, new float?(), str1, "flashing", false);
                }
            }
            else
                Log.w(comm.serialPort.PortName, string.Format("port {0} is not open.", comm.serialPort.PortName));
        }

        private void PropareFirehose()
        {
            ping();
        }

        private void ping()
        {
            Log.w(comm.serialPort.PortName, "send nop command");
            FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, new float?(0.0f), "ping target via firehose", "flashing", false);
            if (!comm.SendCommand(Firehose.Nop, true))
                throw new Exception("ping target failed");
            FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, new float?(1f), "ping target via firehose", "flashing", false);
        }

        private void ConfigureDDR(int intSectorSize, int buffer_sectors, string ddrType, int m_iSkipStorageInit)
        {
            Log.w(comm.serialPort.PortName, "send configure command");
            FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, new float?(0.0f), "send configure command", "flashing", false);
            if (!comm.SendCommand(string.Format(Firehose.Configure, (intSectorSize * buffer_sectors), ddrType, m_iSkipStorageInit), true))
                throw new Exception("send configure command failed");
            FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, new float?(1f), "send command command", "flashing", false);
        }

        private bool Provision(string swpath)
        {
            string[] strArray = FileSearcher.SearchFiles(swpath, SoftwareImage.ProvisionPattern);
            if (strArray.Length == 0)
                return false;
            string str = strArray[0];
            Log.w(comm.serialPort.PortName, string.Format("start provision:{0}", str));
            XmlDocument xmlDocument = new XmlDocument();
            XmlReader reader = XmlReader.Create(str, new XmlReaderSettings()
            {
                IgnoreComments = true
            });
            xmlDocument.Load(reader);
            XmlNodeList childNodes = xmlDocument.SelectSingleNode("data").ChildNodes;
            int num = 0;
            foreach (XmlElement xmlElement in childNodes)
            {
                if (!(xmlElement.Name.ToLower() != "ufs"))
                {
                    StringBuilder stringBuilder = new StringBuilder("<ufs ");
                    foreach (XmlAttribute attribute in xmlElement.Attributes)
                    {
                        if (!(attribute.Name.ToLower() == "desc"))
                            stringBuilder.Append(string.Format("{0}=\"{1}\" ", attribute.Name, attribute.Value));
                    }
                    stringBuilder.Append("/>");
                    string command = string.Format("<?xml version=\"1.0\" ?>\n<data>\n{0}\n</data>", stringBuilder.ToString());
                    if (!comm.SendCommand(command, true))
                        Log.w(comm.serialPort.PortName, "Provision failed :" + command);
                    FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, new float?(num / (float)childNodes.Count), str, "provisioning", false);
                    ++num;
                }
            }
            Log.w(comm.serialPort.PortName, "Provision done.");
            FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, new float?(1f), "provisiong done", "provisioning", false);
            return Reboot(comm.serialPort.PortName);
        }

        private bool Reboot(string portName)
        {
            Log.w(comm.serialPort.PortName, "reboot target");
            FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, new float?(0.0f), "reboot target", "flashing", false);
            if (!comm.SendCommand(Firehose.Reset_To_Edl, true))
                throw new Exception("reboot target failed");
            comm.serialPort.Close();
            comm.serialPort.Dispose();
            List<string> list = ((IEnumerable<string>)getDevice()).ToList();
            int num = 10;
            while (num-- > 0 && list.IndexOf(portName) < 0)
            {
                Thread.Sleep(500);
                list = ((IEnumerable<string>)getDevice()).ToList<string>();
                string str = string.Format("waiting for {0} reboot", portName);
                Log.w(comm.serialPort.PortName, str);
                FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, new float?(), str, "reboot", false);
            }
            bool flag1;
            if (list.IndexOf(portName) >= 0)
            {
                bool flag2 = true;
                string str = string.Format("{0} reboot successfully", portName);
                Log.w(comm.serialPort.PortName, str);
                Thread.Sleep(2000);
                comm.serialPort.Open();
                flag1 = flag2 && comm.IsOpen;
                FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, new float?(1f), str, "reboot", false);
            }
            else
            {
                string str = string.Format("{0} reboot failed", portName);
                Log.w(comm.serialPort.PortName, str);
                FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, new float?(0.0f), str, "reboot", false);
                flag1 = false;
            }
            return flag1;
        }

        private void SetBootPartition()
        {
            FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, new float?(0.0f), "send nop command", "flashing", false);
            if (!comm.SendCommand(Firehose.SetBootPartition, true))
                throw new Exception("set boot partition failed");
            FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, new float?(1f), "set boot partition", "flashing", false);
        }

        private void FirehoseDownloadImg(string swPath)
        {
            string[] strArray1 = FileSearcher.SearchFiles(swPath, SoftwareImage.RawProgramPattern);
            string[] strArray2 = FileSearcher.SearchFiles(swPath, SoftwareImage.PatchPattern);
            for (int index = 0; index < strArray1.Length; ++index)
            {
                if (WriteFilesToDevice(comm.serialPort.PortName, swPath, strArray1[index]))
                    ApplyPatchesToDevice(comm.serialPort.PortName, strArray2[index]);
            }
        }

        private bool WriteFilesToDevice(string portName, string swPath, string rawFilePath)
        {
            bool flag1 = true;
            Log.w(comm.serialPort.PortName, string.Format("open program file {0}", rawFilePath));
            FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, new float?(), rawFilePath, "flashing", false);
            XmlDocument xmlDocument = new XmlDocument();
            XmlReader reader = XmlReader.Create(rawFilePath, new XmlReaderSettings()
            {
                IgnoreComments = true
            });
            xmlDocument.Load(reader);
            XmlNodeList childNodes = xmlDocument.SelectSingleNode("data").ChildNodes;
            bool flag2 = false;
            string str1 = "";
            string str2 = "0";
            string str3 = "0";
            string str4 = "0";
            string str5 = "0";
            string str6 = "512";
            string str7 = "";
            foreach (XmlElement xmlElement in childNodes)
            {
                if (!(xmlElement.Name.ToLower() != "program"))
                {
                    foreach (XmlAttribute attribute in xmlElement.Attributes)
                    {
                        switch (attribute.Name.ToLower())
                        {
                            case "file_sector_offset":
                                str2 = attribute.Value;
                                continue;
                            case "filename":
                                str1 = attribute.Value;
                                continue;
                            case "num_partition_sectors":
                                str4 = attribute.Value;
                                continue;
                            case "start_sector":
                                str3 = attribute.Value;
                                continue;
                            case "sparse":
                                flag2 = attribute.Value == "true";
                                continue;
                            case "sector_size_in_bytes":
                                str6 = attribute.Value;
                                continue;
                            case "physical_partition_number":
                                str5 = attribute.Value;
                                continue;
                            case "label":
                                str7 = attribute.Value;
                                continue;
                            default:
                                continue;
                        }
                    }
                    if (!string.IsNullOrEmpty(str1))
                    {
                        str1 = swPath + "\\" + str1;
                        if (str1.IndexOf("gpt_main1") >= 0 || str1.IndexOf("gpt_main2") >= 0)
                            Thread.Sleep(3000);
                        if (flag2)
                        {
                            Log.w(comm.serialPort.PortName, string.Format("Write sparse file {0} to partition[{1}] sector {2}", str1, str7, str3));
                            new FileTransfer(comm.serialPort.PortName, str1).WriteSparseFileToDevice(this, str3, str4, str1, str2, str6, str5);
                        }
                        else
                        {
                            Log.w(comm.serialPort.PortName, string.Format("Write file {0} to partition[{1}] sector {2}", str1, str7, str3));
                            new FileTransfer(comm.serialPort.PortName, str1).WriteFile(this, str3, str4, str1, str2, "0", str6, str5);
                        }
                        Log.w(comm.serialPort.PortName, string.Format("Image {0} transferred successfully", str1));
                    }
                }
            }
            reader.Close();
            return flag1;
        }

        private bool ApplyPatchesToDevice(string portName, string patchFilePath)
        {
            bool flag = true;
            FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, new float?(0.0f), patchFilePath, "flashing", false);
            XmlDocument xmlDocument = new XmlDocument();
            XmlReader reader = XmlReader.Create(patchFilePath, new XmlReaderSettings()
            {
                IgnoreComments = true
            });
            xmlDocument.Load(reader);
            XmlNodeList childNodes = xmlDocument.SelectSingleNode("patches").ChildNodes;
            string str = "";
            string pszPatchSize = "0";
            string pszPatchValue = "0";
            string pszDiskOffsetSector = "0";
            string pszSectorOffsetByte = "0";
            string pszPhysicalPartitionNumber = "0";
            string pszSectorSizeInBytes = "512";
            foreach (XmlElement xmlElement in childNodes)
            {
                if (!(xmlElement.Name.ToLower() != "patch"))
                {
                    foreach (XmlAttribute attribute in xmlElement.Attributes)
                    {
                        switch (attribute.Name.ToLower())
                        {
                            case "byte_offset":
                                pszSectorOffsetByte = attribute.Value;
                                continue;
                            case "filename":
                                str = attribute.Value;
                                continue;
                            case "size_in_bytes":
                                pszPatchSize = attribute.Value;
                                continue;
                            case "start_sector":
                                pszDiskOffsetSector = attribute.Value;
                                continue;
                            case "value":
                                pszPatchValue = attribute.Value;
                                continue;
                            case "sector_size_in_bytes":
                                pszSectorSizeInBytes = attribute.Value;
                                continue;
                            case "physical_partition_number":
                                pszPhysicalPartitionNumber = attribute.Value;
                                continue;
                            default:
                                continue;
                        }
                    }
                    if (str.ToLower() == "disk")
                        ApplyPatch(pszDiskOffsetSector, pszSectorOffsetByte, pszPatchValue, pszPatchSize, pszSectorSizeInBytes, pszPhysicalPartitionNumber);
                }
            }
            FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, new float?(1f), patchFilePath, "flashing", false);
            return flag;
        }

        private void ApplyPatch(string pszDiskOffsetSector, string pszSectorOffsetByte, string pszPatchValue, string pszPatchSize, string pszSectorSizeInBytes, string pszPhysicalPartitionNumber)
        {
            Log.w(comm.serialPort.PortName, string.Format("ApplyPatch sector {0}, offset {1}, value {2}, size {3}", pszDiskOffsetSector, pszSectorOffsetByte, pszPatchValue, pszPatchSize));
            comm.SendCommand(string.Format(Firehose.FIREHOSE_PATCH, pszSectorSizeInBytes, pszSectorOffsetByte, pszPhysicalPartitionNumber, pszPatchSize, pszDiskOffsetSector, pszPatchValue));
        }

        public override string[] getDevice()
        {
            return ComPortCtrl.getDevicesQc();
        }
    }
}
