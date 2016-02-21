namespace PSU.NocSym.Core
{
    public class WireEnd
    {
        public WireEnd OppositeEnd { get; set; }
        public Node Node { get; set; }
        public Wire Wire { get; set; }
        public bool IsConnected { get { return Node != null; } }

        public WireEnd(Wire wire)
        {
            Wire = wire;
        }        
    }
}
