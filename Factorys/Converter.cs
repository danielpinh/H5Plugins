using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H5Plugins.Factorys
{
    class Converter
    {
        public double MetertoFeet (double meterValue)
        {
            double feetValue;
            double coefficient= 3.281;
            feetValue = meterValue * coefficient;
            return feetValue;
        }

        public double FeettoMeter(double feetValue)
        {
            double meterValue;
            double coefficient = 3.281;
            meterValue = feetValue / coefficient;
            return meterValue;
        }
    }
}
