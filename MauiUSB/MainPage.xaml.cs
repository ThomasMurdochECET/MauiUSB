using FTD2XX_NET;
using System.Diagnostics;

namespace MauiUSB

{
    public partial class MainPage : ContentPage
    {
        UInt32 ftdiDeviceCount = 0;
        FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;

        // Allocate storage for device info list
        FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[64];

        public MainPage()
        {
            InitializeComponent();
        }

        private void BtnOpenClose_Clicked(object sender, EventArgs e)
        {
            RefreshDeviceList();
           SetupFTDIdeviceByAutoSerialNumber();
        }

        private void SetupFTDIdeviceByAutoSerialNumber()
        {
            ftStatus = myFtdiDevice.OpenBySerialNumber(ftdiDeviceList[0].SerialNumber);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Wait for a key press
                Console.WriteLine("Failed to open device (error " + ftStatus.ToString() + ")");
                Console.ReadKey();
                return;
            }

            // Set up device data parameters
            // Set Baud rate to 9600
            ftStatus = myFtdiDevice.SetBaudRate(9600);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Wait for a key press
                Console.WriteLine("Failed to set Baud rate (error " + ftStatus.ToString() + ")");
                Console.ReadKey();
                return;
            }

            // Set data characteristics - Data bits, Stop bits, Parity
            ftStatus = myFtdiDevice.SetDataCharacteristics(FTDI.FT_DATA_BITS.FT_BITS_8, FTDI.FT_STOP_BITS.FT_STOP_BITS_1, FTDI.FT_PARITY.FT_PARITY_NONE);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Wait for a key press
                Console.WriteLine("Failed to set data characteristics (error " + ftStatus.ToString() + ")");
                Console.ReadKey();
                return;
            }

            // Set flow control - set RTS/CTS flow control
            ftStatus = myFtdiDevice.SetFlowControl(FTDI.FT_FLOW_CONTROL.FT_FLOW_RTS_CTS, 0x11, 0x13);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Wait for a key press
                Console.WriteLine("Failed to set flow control (error " + ftStatus.ToString() + ")");
                Console.ReadKey();
                return;
            }

            // Set read timeout to 5 seconds, write timeout to infinite
            ftStatus = myFtdiDevice.SetTimeouts(5000, 0);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Wait for a key press
                Console.WriteLine("Failed to set timeouts (error " + ftStatus.ToString() + ")");
                Console.ReadKey();
                return;
            }
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

        }
    }

}
