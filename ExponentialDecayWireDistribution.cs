using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSU.NocSym.Core
{
    public class ExponentialDecayWireDistribution : IWireDistribution
    {
        private double CurveScalar { get; set; }   
        private double MaxWireLength { get; set; }        
        private Func<double, double> CurveEquation { get; set; }        

        public ExponentialDecayWireDistribution(double maxWireLength, double curveScalar = 5)
        {           
            MaxWireLength = maxWireLength;
            CurveScalar = curveScalar;            
            CurveEquation = x => MaxWireLength * Math.Exp(-x / (MaxWireLength / CurveScalar));
            
        }

        public bool IsAboveControl(double length)
        {
            return CurveEquation(length) <= (new Random()).NextDouble() * MaxWireLength;
        }
    }
}
