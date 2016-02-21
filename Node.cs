using System;
using System.Collections.Generic;
using System.Linq;

namespace PSU.NocSym.Core
{
    public class Node : IComparable<Node>
    {
        public List<WireEnd> Connections { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }

        public Node(int row, int column)
        {
            Row = row;
            Column = column;
            Connections = new List<WireEnd>();
        }

        public Wire Connect(Wire wire)
        {
            var end = wire.Connect();
            end.Node = this;
            Connections.Add(end);

            return wire;
        }

        public bool IsConnectedTo(Node node)
        {
            return Connections.Any(e => e.OppositeEnd.Node == node);
        }

        public int CompareTo(Node other)
        {
            if (this.Row < other.Row)
            {
                return -1;
            }

            if (this.Row == other.Row) 
            {
                if(this.Column < other.Column)
                {
                    return -1;
                }

                if (this.Column == other.Column)
                {
                    return 0;
                }
            }

            return 1;            
        }
    }
}
