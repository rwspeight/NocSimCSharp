using QuickGraph;
using System;

namespace PSU.NocSym.Core
{
    public class Wire : IEdge<Node>
    {
        public WireEnd EndA { get; private set; }
        public WireEnd EndB { get; private set; }
        public double Length { get; set; }

        public Wire(double length)
        {
            Length = length;
            EndA = new WireEnd(this);
            EndB = new WireEnd(this);

            EndA.OppositeEnd = EndB;
            EndB.OppositeEnd = EndA;            
        }

        public WireEnd Connect()
        {
            if (!EndA.IsConnected)
            {
                return EndA;
            }
            else if (!EndB.IsConnected)
            {
                return EndB;
            }
            else
            {
                throw new InvalidOperationException("Both ends of the wire are connected.");
            }            
        }

        public Node Source
        {
            get { return EndA.Node; }
        }

        public Node Target
        {
            get { return EndB.Node; }
        }
    }
}
