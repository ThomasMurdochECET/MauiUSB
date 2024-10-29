﻿using FTD2XX_NET;
using Microsoft.Maui.Layouts;
using System.Diagnostics;
using System.Text;

namespace MauiUSB

{
    public partial class MainPage : ContentPage
    {
        UInt32 ftdiDeviceCount = 0;
        FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;

        // Allocate storage for device info list
        FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[64];

        // Create new instance of the FTDI device class
        FTDI myFtdiDevice = new FTDI();
        //Variable to store data size
        private uint packetSize = 39;
        private uint numBytesWritten;
        private bool sendDataOK;
        private bool bPortOpen;
        private string newPacket;
        private int newPacketNumber;
       

        public MainPage()
        {
            InitializeComponent();
        }

        private void BtnOpenClose_Clicked(object sender, EventArgs e)
        {
            if (!bPortOpen)
            {
                RefreshDeviceList();
                sendDataOK = false; // set the sendDataOK flag to false
                SetupFTDIdeviceByAutoSerialNumber();
                _ = MonitorUSBData(); // Monitor the USB data asynchronously
                BtnOpenClose.Text = "Close";
                bPortOpen = true;
            }
            else
            {
                CloseFTDIdevice();
                BtnOpenClose.Text = "Open";
                bPortOpen = false;
            }
            
        }

        private void CloseFTDIdevice()
        {
            ftStatus = myFtdiDevice.Close();
        }

        private async Task MonitorUSBData()
        {
            while (myFtdiDevice.IsOpen)
            {
                // Check amoun of data available to read
                //In this case we know how much data we are expecting
                //so wait until we have all of the bytes we have sent.
                UInt32 numBytesAvailable = 0;
                do
                {
                    ftStatus = myFtdiDevice.GetRxBytesAvailable(ref numBytesAvailable);
                    if (ftStatus != FTDI.FT_STATUS.FT_OK)
                    {
                        // Wait for a key press
                        Trace.WriteLine("Failed to get number of bytes available to read (error " + ftStatus.ToString() + ")");
                        return;
                    }
                    Thread.Sleep(10);
                }
                while (numBytesAvailable < packetSize);

                // Now that we have the amount of data we want available, read it
                string readData;
                UInt32 numBytesRead = 0;
                // Note that the Read method is overloaded, so can read string or byte array data
                ftStatus = myFtdiDevice.Read(out readData, numBytesAvailable, ref numBytesRead);
                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                {
                    // Wait for a key press
                    Trace.WriteLine("Failed to read data (error " + ftStatus.ToString() + ")");
                    return;
                }
                Trace.WriteLine("Read data:" + readData);
                newPacket = readData;
                if (checkBoxHistory.IsChecked == true)
                {
                    LabelRXdata.Text = newPacket +LabelRXdata.Text;

                }
                else
                {
                    LabelRXdata.Text = newPacket;
                }
                await Task.Delay(90);
                if(newPacket.Length > packetSize)
                {
                    ParseNewPacket();
                }

            }
        }

        private void ParseNewPacket()
        {
            if(newPacket.Substring(0,3)=="###")
            {
                int calChkSum =0;
                newPacketNumber = Convert.ToInt32(newPacket.Substring(3, 3));

                for (int i = 3; i<34; i++)
                {
                    calChkSum += (byte)newPacket[i];
                }
                calChkSum %= 1000;
                int recChkSum = Convert.ToInt32(newPacket.Substring(34, 3));
                if(calChkSum == recChkSum)
                {
                    Trace.WriteLine("Packet number:"+newPacketNumber);
                    Trace.WriteLine("Checksum: " + recChkSum);
                    Trace.WriteLine("Data: " + newPacket.Substring(3, 31));
                    string parsedData = $"{newPacket.Length,-14}" +
                                        $"{newPacket.Substring(0,  3),-14}"+
                                        $"{newPacket.Substring(6,  4),-14}"+
                                        $"{newPacket.Substring(10, 4),-14}"+
                                        $"{newPacket.Substring(14, 4),-14}"+
                                        $"{newPacket.Substring(18, 4),-14}"+
                                        $"{newPacket.Substring(22, 4),-14}"+
                                        $"{newPacket.Substring(26, 4),-14}"+
                                        $"{newPacket.Substring(30, 4),-14}"+
                                        $"{newPacket.Substring(34, 3),-14}"+
                                        $"{calChkSum,-14}" + "\r\n";
                    
                    if(checkBoxParsedHistory.IsChecked == true)
                    {
                        LabelParsedData.Text = parsedData + LabelParsedData.Text;
                    }
                    else
                    {
                        LabelParsedData.Text = parsedData;
                    }
                                        

                             

                }
                else
                {
                    Trace.WriteLine("Checksum error");
                }

            }
        }

        private void SetupFTDIdeviceByAutoSerialNumber()
        {
            ftStatus = myFtdiDevice.OpenBySerialNumber(ftdiDeviceList[0].SerialNumber);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Wait for a key press
                Trace.WriteLine("Failed to open device (error " + ftStatus.ToString() + ")");
                 
                return;
            }

            // Set up device data parameters
            // Set Baud rate to 115200
            ftStatus = myFtdiDevice.SetBaudRate(115200);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Wait for a key press
                Trace.WriteLine("Failed to set Baud rate (error " + ftStatus.ToString() + ")");
                 
                return;
            }

            // Set data characteristics - Data bits, Stop bits, Parity
            ftStatus = myFtdiDevice.SetDataCharacteristics(FTDI.FT_DATA_BITS.FT_BITS_8, FTDI.FT_STOP_BITS.FT_STOP_BITS_1, FTDI.FT_PARITY.FT_PARITY_NONE);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Wait for a key press
                Trace.WriteLine("Failed to set data characteristics (error " + ftStatus.ToString() + ")");
                 
                return;
            }

           
            // Set read timeout to 5 seconds, write timeout to infinite
            ftStatus = myFtdiDevice.SetTimeouts(5000, 0);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Wait for a key press
                Trace.WriteLine("Failed to set timeouts (error " + ftStatus.ToString() + ")");
                 
                return;
            }

            // Check the amount of data available to read
            // In this case we know how much data we are expecting, 
            // so wait until we have all of the bytes we have sent.
            uint numBytesAvailable = 0;
            do
            {
                ftStatus = myFtdiDevice.GetRxBytesAvailable(ref numBytesAvailable);
                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                {
                    // Wait for a key press
                    Trace.WriteLine("Failed to get number of bytes available to read (error " + ftStatus.ToString() + ")");
                     
                    return;
                }
               Thread.Sleep(90);
            } while (numBytesAvailable < packetSize);

            // Now that we have the amount of data we want available, read it
            string readData;
            UInt32 numBytesRead = 0;
            // Note that the Read method is overloaded, so can read string or byte array data
            ftStatus = myFtdiDevice.Read(out readData, numBytesAvailable, ref numBytesRead);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Wait for a key press
                Trace.WriteLine("Failed to read data (error " + ftStatus.ToString() + ")");
                 
                return;
            }
            
            Trace.WriteLine("Read data:" + readData);
        }

        private void RefreshDeviceList()
        {
            // Determine the number of FTDI devices connected to the machine
            ftStatus = myFtdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);
            // Check status
            if (ftStatus == FTDI.FT_STATUS.FT_OK)
            {
               Trace.WriteLine("Number of FTDI devices: " + ftdiDeviceCount.ToString());
               Trace.WriteLine("");
            }
            else
            {
                // Wait for a key press
               Trace.WriteLine("Failed to get number of devices (error " + ftStatus.ToString() + ")");
                
                return;
            }

            // If no devices available, return
            if (ftdiDeviceCount == 0)
            {
                // Wait for a key press
               Trace.WriteLine("Failed to get number of devices (error " + ftStatus.ToString() + ")");
                
                return;
            }

            // Populate our device list
            ftStatus = myFtdiDevice.GetDeviceList(ftdiDeviceList);

            if (ftStatus == FTDI.FT_STATUS.FT_OK)
            {
                for (UInt32 i = 0; i < ftdiDeviceCount; i++)
                {
                   Trace.WriteLine("Device Index: " + i.ToString());
                   Trace.WriteLine("Flags: " + String.Format("{0:x}", ftdiDeviceList[i].Flags));
                   Trace.WriteLine("Type: " + ftdiDeviceList[i].Type.ToString());
                   Trace.WriteLine("ID: " + String.Format("{0:x}", ftdiDeviceList[i].ID));
                   Trace.WriteLine("Location ID: " + String.Format("{0:x}", ftdiDeviceList[i].LocId));
                   Trace.WriteLine("Serial Number: " + ftdiDeviceList[i].SerialNumber.ToString());
                   Trace.WriteLine("Description: " + ftdiDeviceList[i].Description.ToString());
                   Trace.WriteLine("");
                }
            }

        }

        private void BtnClear_Clicked(object sender, EventArgs e)
        {

        }

        private void BtnSend_Clicked(object sender, EventArgs e)
        {
            try
            {
                string messageOut = entrySend.Text;
                messageOut += "\r\n";
                byte[] messageBytes = Encoding.ASCII.GetBytes(messageOut);
                ftStatus = myFtdiDevice.Write(messageBytes, messageBytes.Length, ref numBytesWritten);
                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                {
                    // Wait for a key press
                    Trace.WriteLine("Failed to write to device (error " + ftStatus.ToString() + ")");
                    return;
                }

            }
            catch (Exception ex) 
            {
                Trace.WriteLine(ex.Message);
            }


        }    
    }

}
