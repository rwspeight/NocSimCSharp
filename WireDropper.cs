using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PSU.NocSym.Core
{
    /// <summary>
    /// Implementation of the wire dropping logic.  This is a idealized wire
    /// drop where a random wire and a random node are selected.  If the wire length
    /// will not connect that node to any other nodes then we skip it and pull another
    /// wire.  Statistically this is as if we dropped the wire and it didn't connect anything.
    /// If the selected wire can connect nodes then we randomly select from the valid nodes.
    /// This is acceptable as it is as if we dropped a hundred wires that didn't connect to 
    /// finally get one that does.  (Note to self:  I should prove this mathematically)
    /// </summary>
    public class WireDropper
    {
        private const string Log = @"Z:\Repos\NanowireNetwork\dist.csv";
        private Random Random { get; set; }
        private IWireDistribution Distribution { get; set; }
        private SquareLattice Lattice { get; set; }
        private List<Tuple<double, double>> LengthMap { get; set; }
        public WireDropper(SquareLattice lattice, IWireDistribution distribution)            
        {
            Distribution = distribution;
            Lattice = lattice;
            Random = new Random();
            LengthMap = CreateLengthMap(Lattice.DistanceToNodes
                                                     .Select(d => new Tuple<double,int>(d.Key, d.Value.Count)))
                .OrderBy(l => l.Item1)
                .ToList();
            /*
            File.WriteAllLines(
                @"Z:\Repos\NanowireNetwork\map.csv",
                Lattice.DistanceToNodes.SelectMany(d => Enumerable.Repeat(d.Key.ToString(), d.Value.Count)));
            File.Delete(Log);            
             */
        }

        /// <summary>
        /// Create a mapping between a value between 0.0 and 1.0 and one of the wire lengths.
        /// Each wire will occupy an arbitrary sub-interval between 0.0 and 1.0.  This is used to
        /// power a random selector with the goal of the wire frequency powering the probability.
        /// </summary>
        /// <param name="lengths"></param>
        /// <returns></returns>
        private List<Tuple<double, double>> CreateLengthMap(IEnumerable<Tuple<double, int>> lengths)
        {
            double total = lengths.Sum(l => l.Item2);
            double offset = 0;

            return lengths.Select(l =>
            {
                var t = new Tuple<double, double>(offset + (double)l.Item2 / total, l.Item1);
                offset = t.Item1;
                return t;
            }).ToList();            
        }

        public void Drop()
        {
            double length = 0;
            do
            {
                // Generate the lookup outside the lambda to prevent potential for multiple calls.
                var lookup = Random.NextDouble();
                length = LengthMap.Where(l => l.Item1 >= lookup)
                                  .Select(l => l.Item2)
                                  .First();
                
            }
            while(Distribution.IsAboveControl(length));

            File.AppendAllLines(Log, new[] { length.ToString() });

            SquareLattice.InterNodeDistance pair = null;
            var nodePairs = Lattice.DistanceToNodes[length]
                                   .Where(p => !p.FromNode.IsConnectedTo(p.ToNode))
                                   .ToList();
            if (nodePairs.Count > 0)
            {
                pair = nodePairs[Random.Next(0, nodePairs.Count - 1)];

                var wire = new Wire(length);
                pair.FromNode.Connect(wire);
                pair.ToNode.Connect(wire);
            }
        }
    }
}
