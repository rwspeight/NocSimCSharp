using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSU.NocSym.Core
{
    /// <summary>
    /// Approximates a linearly decreases distrubtion where small values have the 
    /// highest probability and large values the lowest.
    /// </summary>
    public class LinearWireDistribution : IWireDistribution
    {
        private double MinX { get; set; }
        private double MaxX { get; set; }
        private double MaxY { get; set; }
        private double DeltaSquare { get; set; }
        private Func<double, double> LineEquation { get; set; }        

        public LinearWireDistribution(double min, double max)
        {
            MinX = min;
            MaxX = max;
            MaxY = 2 / (max - min);
            DeltaSquare = Math.Pow(MaxX - MinX, 2);
            LineEquation = x => -2 * x / DeltaSquare + 2 * MaxX / DeltaSquare;            
        }

        public bool IsAboveControl(double length)
        {
            return LineEquation(length) <= (new Random()).NextDouble() * MaxY;
        }
    }
}
