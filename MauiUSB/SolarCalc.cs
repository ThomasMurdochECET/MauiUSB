using NetworkExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MauiUSB
{
    
    public class SolarCalc
    {
        string[] analogVoltage = new string [6];
        private double shuntResistorAnalog = 100; // resistor value used in circuit

        public string GetCurrent(double an1, double shuntResistorAnalog)
        {
            return analogVoltage[1] = Convert.ToString(an1 * shuntResistorAnalog);
        }

        public string GetLEDCurrent(object value1, object value2)
        {
            
        }

        public string GetVoltage(double analogValue)
        {
            return analogVoltage[analogValue] = Convert.ToString(analogValue * shuntResistorAnalog);
        }

        public void ParseSolarData(string ValidPacket)  // takes new valid packet and parses it to double values, to be used for calcs
        {
            
            
            double an0 = Convert.ToDouble (ValidPacket.Substring(6, 4));  // Analog 0
            double an1 = Convert.ToDouble (ValidPacket.Substring(10, 4)); // Analog 1
            double an2 = Convert.ToDouble (ValidPacket.Substring(14, 4)); // Analog 2
            double an3 = Convert.ToDouble (ValidPacket.Substring(18, 4)); // Analog 3
            double an4 = Convert.ToDouble (ValidPacket.Substring(22, 4)); // Analog 4
            double an5 = Convert.ToDouble (ValidPacket.Substring(26, 4)); // Analog 5
        }

       

         
       
    
    
    }





}
  
              
              
        
    
}
