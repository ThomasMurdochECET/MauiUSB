//using NetworkExtension;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MauiUSB
{

    public class SolarCalc
    {
        public double[] analogVoltage = new double[5];


        public string GetCurrent(double analogValue1, double analogValue2)
        {
            return ((analogValue1 - analogValue2) / 100).ToString("F3");

        }

        public string GetLEDCurrent(double analogValue1, double VoltageToLED)
        {
            return ((analogValue1 - VoltageToLED) / 100).ToString("F3");
        }

        public string GetVoltage(double analogValue)
        {
            return (analogValue / 1000).ToString("F3"); ;
        }

        public void ParseSolarData(string ValidPacket)  // takes new valid packet and parses it to double values, to be used for calcs
        {


            analogVoltage[0] = Convert.ToDouble(ValidPacket.Substring(6, 4));  // Analog 0
            analogVoltage[1] = Convert.ToDouble(ValidPacket.Substring(10, 4)); // Analog 1
            analogVoltage[2] = Convert.ToDouble(ValidPacket.Substring(14, 4)); // Analog 2
            analogVoltage[3] = Convert.ToDouble(ValidPacket.Substring(18, 4)); // Analog 3
            analogVoltage[4] = Convert.ToDouble(ValidPacket.Substring(22, 4)); // Analog 4


        }







    }





}






