using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSU.NocSym.Core
{
    public class WireSelector
    {
        private Random Random { get; set; }
        public IList<double> Lengths { get; set; }
        
        /// <summary>
        /// Really just the PDF.
        /// </summary>
        public Func<double, bool> ControlFunction { get; set; }
        public WireSelector(IList<double> lengths, Func<double, bool> control)
        {
            ControlFunction = control;
            Lengths = lengths;
            Random = new Random();
        }

        public Wire Select()
        {
            double length = 0;
            do
            {
                length = Lengths[Random.Next(0, Lengths.Count - 1)];
            }
            while (!ControlFunction(length));

            return new Wire(length);
        }
    }
}
