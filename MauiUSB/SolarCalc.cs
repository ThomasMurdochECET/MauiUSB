//using NetworkExtension;
namespace MauiUSB
{
    //for the following class we calculate the voltages and currents using an analog input and shunt resistors, we then use 
    public class SolarCalc
    {
        public double[] analogVoltage = new double[5];

        public string GetCurrent(double analogValue1, double analogValue2)
        {
            return ((analogValue2 - analogValue1) / 100).ToString("  0.000 mA; -0.000 mA;  0.000 mA");
        }

        public string GetLEDCurrent(double analogValue1, double VoltageToLED)
        {
            if ((analogValue1 - VoltageToLED) >= 0)
            {
                return (((analogValue1 - VoltageToLED) / 100)).ToString("  0.000 mA; -0.000 mA;  0.000 mA");
            }

            else
            {
                return ("  0.000 mA"); //ensures we dont see a negative value at the LEDs
            }

        }

        public string GetVoltage(double analogValue)
        {
            return (analogValue / 1000).ToString("  0.000 V; -0.000 V;  0.000 V"); 
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