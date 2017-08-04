

using System;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
using XiaoMiFlash.code.data;
using XiaoMiFlash.code.module;
using XiaoMiFlash.code.Utility;

namespace XiaoMiFlash.code.bl
{
  public class FileTransfer
  {
    protected FileStream fileStream;
    public string filePath;
    public string portName;
    private long fileLength;

    public FileTransfer(string port, string filePath)
    {
      portName = port;
      this.filePath = filePath;
      FlashingDevice.UpdateDeviceStatus(portName, new float?(0.0f), "flashing " + filePath, "flashing", false);
      openFile(filePath);
    }

    ~FileTransfer()
    {
      if (fileStream == null)
        return;
      fileStream.Close();
      fileStream.Dispose();
      Log.w(portName, "destruct close file " + filePath);
    }

    private bool openFile(string filePath)
    {
      this.filePath = filePath;
      try
      {
        fileLength = new FileInfo(filePath).Length;
        fileStream = File.OpenRead(filePath);
        return true;
      }
      catch (Exception )
      {
        return false;
      }
    }

    public int transfer(SerialPort port, int offset, int size)
    {
      if (!port.IsOpen)
      {
        string.Format("{0} is not open", port.PortName);
        return 0;
      }
      int n = 0;
      byte[] bytesFromFile = GetBytesFromFile(offset, size, out n);
      port.Write(bytesFromFile, 0, size);
      return n;
    }

    public void WriteFile(SerialPortDevice portCnn, string strPartitionStartSector, string strPartitionSectorNumber, string pszImageFile, string strFileStartSector, string strFileSectorOffset, string sector_size, string physical_partition_number)
    {
      long num1 = Convert.ToInt64(strPartitionSectorNumber);
      if (num1 == 0L)
        num1 = int.MaxValue;
      long int64_1 = Convert.ToInt64(strFileStartSector);
      long int64_2 = Convert.ToInt64(strFileSectorOffset);
      long int64_3 = Convert.ToInt64(sector_size);
      long int64_4 = Convert.ToInt64(physical_partition_number);
      Log.w(portCnn.comm.serialPort.PortName, string.Format("write file {0} legnth {1} to partition {2}", filePath, getFileSize(), strPartitionStartSector));
      long num2 = (getFileSize() + int64_3 - 1L) / int64_3;
      if (num2 - int64_1 > num1)
        num2 = int64_1 + num1;
      else
        num1 = num2 - int64_1;
      string str = string.Format(Firehose.FIREHOSE_PROGRAM, int64_3, num1, strPartitionStartSector, int64_4);
      portCnn.comm.SendCommand(str);
      Log.w(portCnn.comm.serialPort.PortName, str);
      long num3 = int64_1;
      while (num3 < num2)
      {
        long num4 = num2 - num3;
        long num5 = num4 < portCnn.comm.m_dwBufferSectors ? num4 : portCnn.comm.m_dwBufferSectors;
        Log.w(portCnn.comm.serialPort.PortName, string.Format("WriteFile position {0}, size {1}", (int64_3 * num3).ToString("X"), int64_3 * num5));
        long offset = int64_2 + int64_3 * num3;
        int size = (int) (int64_3 * num5);
        int n = 0;
        byte[] bytesFromFile = GetBytesFromFile(offset, size, out n);
        portCnn.comm.WritePort(bytesFromFile, 0, bytesFromFile.Length);
        num3 += portCnn.comm.m_dwBufferSectors;
      }
    }

    public void WriteSparseFileToDevice(SerialPortDevice portCnn, string pszPartitionStartSector, string pszPartitionSectorNumber, string pszImageFile, string pszFileStartSector, string pszSectorSizeInBytes, string pszPhysicalPartitionNumber)
    {
      int int32_1 = Convert.ToInt32(pszPartitionStartSector);
      int int32_2 = Convert.ToInt32(pszPartitionSectorNumber);
      int int32_3 = Convert.ToInt32(pszFileStartSector);
      long offset1 = 0;
      int int32_4 = Convert.ToInt32(pszSectorSizeInBytes);
      Convert.ToInt32(pszPhysicalPartitionNumber);
      SparseImageHeader sparseImageHeader = new SparseImageHeader();
      string str = "";
      if (int32_3 != 0)
      {
        str = "ERROR_BAD_FORMAT";
        Log.w(portCnn.comm.serialPort.PortName, str);
      }
      if (int32_4 == 0)
      {
        str = "ERROR_BAD_FORMAT";
        Log.w(portCnn.comm.serialPort.PortName, "ERROR_BAD_FORMAT");
      }
      int size1 = Marshal.SizeOf(sparseImageHeader);
      int n = 0;
      SparseImageHeader stuct1 = (SparseImageHeader) CommandFormat.BytesToStuct(GetBytesFromFile(offset1, size1, out n), typeof (SparseImageHeader));
      long offset2 = offset1 + stuct1.uFileHeaderSize;
      if ((int) stuct1.uMagic != -316211398)
      {
        str = "ERROR_BAD_FORMAT";
        Log.w(portCnn.comm.serialPort.PortName, "ERROR_BAD_FORMAT");
      }
      if (stuct1.uMajorVersion != 1)
      {
        str = "ERROR_UNSUPPORTED_TYPE";
        Log.w(portCnn.comm.serialPort.PortName, "ERROR_UNSUPPORTED_TYPE");
      }
      if (stuct1.uBlockSize % int32_4 != 0L)
      {
        str = "ERROR_BAD_FORMAT";
        Log.w(portCnn.comm.serialPort.PortName, "ERROR_BAD_FORMAT");
      }
      if (int32_2 != 0 && stuct1.uBlockSize * stuct1.uTotalBlocks / int32_4 > int32_2)
      {
        str = "ERROR_FILE_TOO_LARGE";
        Log.w(portCnn.comm.serialPort.PortName, "ERROR_FILE_TOO_LARGE");
      }
      if (!string.IsNullOrEmpty(str))
      {
        FlashingDevice.UpdateDeviceStatus(portName, new float?(), str, "error", false);
      }
      else
      {
        for (int index = 0; index < stuct1.uTotalChunks; ++index)
        {
          Log.w(portCnn.comm.serialPort.PortName, string.Format("total chunks {0}, current chunk {1}", stuct1.uTotalChunks, index));
          int size2 = Marshal.SizeOf(new SparseChunkHeader());
          SparseChunkHeader stuct2 = (SparseChunkHeader) CommandFormat.BytesToStuct(GetBytesFromFile(offset2, size2, out n), typeof (SparseChunkHeader));
          offset2 += stuct1.uChunkHeaderSize;
          int num1 = (int) stuct1.uBlockSize * (int) stuct2.uChunkSize;
          int num2 = num1 / int32_4;
          switch (stuct2.uChunkType)
          {
            case 51905:
              if (stuct2.uTotalSize != stuct1.uChunkHeaderSize + num1)
              {
                Log.w(portCnn.comm.serialPort.PortName, "ERROR_BAD_FORMAT");
                FlashingDevice.UpdateDeviceStatus(portName, new float?(), "ERROR_BAD_FORMAT", "error", false);
                return;
              }
              string strPartitionStartSector = int32_1.ToString();
              string strPartitionSectorNumber = num2.ToString();
              string strFileStartSector = (offset2 / int32_4).ToString();
              string strFileSectorOffset = (offset2 % int32_4).ToString();
              WriteFile(portCnn, strPartitionStartSector, strPartitionSectorNumber, pszImageFile, strFileStartSector, strFileSectorOffset, pszSectorSizeInBytes, pszPhysicalPartitionNumber);
              offset2 += int32_4 * num2;
              int32_1 += num2;
              break;
            case 51907:
              if ((int) stuct2.uTotalSize != stuct1.uChunkHeaderSize)
                Log.w(portCnn.comm.serialPort.PortName, "ERROR_BAD_FORMAT");
              int32_1 += num2;
              break;
            default:
              Log.w(portCnn.comm.serialPort.PortName, "ERROR_UNSUPPORTED_TYPE");
              break;
          }
        }
      }
    }

    private byte[] GetBytesFromFile(long offset, int size, out int n)
    {
      long length = fileStream.Length;
      byte[] buffer = new byte[size];
      fileStream.Seek(offset, SeekOrigin.Begin);
      n = fileStream.Read(buffer, 0, size);
      if (offset > length || offset + size > length)
      {
        fileStream.Close();
        fileStream.Dispose();
        Log.w(portName, "close file " + filePath);
      }
      FlashingDevice.UpdateDeviceStatus(portName, new float?(offset / (float)length), null, "flashing", false);
      return buffer;
    }

    private long getFileSize()
    {
      if (fileLength != 0L)
        return fileLength;
      return new FileInfo(filePath).Length;
    }
  }
}
