using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PSU.NocSym.Core
{
    public class SquareLattice
    {
        public int Width { get; set; }
        public IList<double> Lengths { get; set; }

        public List<Node> Nodes { get; set; }
        public Dictionary<Node, List<NodeDistance>> NodeToDistances { get; set; }
        public Dictionary<double, List<InterNodeDistance>> DistanceToNodes { get; set; }
        public List<double> AllowLengths { get; set; }

        public SquareLattice(int width)
        {
            Width = width;
            //Lengths = GenerateLengths();

            Populate();
        }

        private void Populate()
        {
            Nodes = Enumerable.Range(0, Width)
                              .SelectMany(row => Enumerable.Range(0, Width)
                                                           .Select(column => new Node(row, column)))
                              .ToList();

            // Calculate distances between all nodes.  This is n*(n-1)/2 calculations.
            Func<Node, Node, double> calculateDistance = (n1, n2) => Math.Sqrt(
                                                            Math.Pow(n1.Row - n2.Row, 2)
                                                          + Math.Pow(n1.Column - n2.Column, 2));
            // #& Think of a better temporary variables name other than reusing distances
            var distances =
            Nodes.ToArray()
                 .SelectMany((n1, i) => Nodes.Skip(i + 1)
                                            .Select(n2 => new InterNodeDistance
                                            {
                                                FromNode = n1,
                                                ToNode = n2,
                                                Distance = calculateDistance(n1, n2)
                                            }))
                 .ToList();

            DistanceToNodes = 
            distances.GroupBy(d => d.Distance)
                     .ToDictionary(g => g.Key,
                                   g => g.ToList());

            Func<List<InterNodeDistance>, Func<InterNodeDistance, Node>, Func<InterNodeDistance, Node>, Dictionary<Node, List<NodeDistance>>> generateDictionary =
            (list, groupBy, node) =>
            {
                return list.GroupBy(groupBy)
                    .ToDictionary(g => g.Key,
                                  g => g.Select(e => new NodeDistance
                                  {
                                      Node = node(e),
                                      Distance = e.Distance
                                  }).ToList());
            };            

            NodeToDistances = generateDictionary(distances, i => i.FromNode, i => i.ToNode);

            // LINQ Problem:  Merge two dictionarys where the value is a List<>.
            // Merge the dictionaries by key.
            foreach (var entry in generateDictionary(distances, i => i.ToNode, i => i.FromNode))
            {
                List<NodeDistance> list = null;
                if (!NodeToDistances.TryGetValue(entry.Key, out list))
                {
                    NodeToDistances.Add(entry.Key, entry.Value);
                }
                else
                {
                    list.AddRange(entry.Value);
                }
            }

            AllowLengths = 
                NodeToDistances.Values
                         .SelectMany(v => v.Select(n => n.Distance))
                         .Distinct()
                         .OrderBy(d => d)
                         .ToList();
        }

        private IList<double> GenerateLengths()
        {
            var result = new List<double>();

            // We generate all the possible lengths between a grid of integral
            // distances.  We do this via Pythagorean formula and only calculate
            // for unique pairs of A and B (A^2 + B^2 = C^2).  We filter out two
            // points (0,0) and (MaxColumn,MaxColumn + 1)
            return
            Enumerable.Range(0, Width)
                      .SelectMany(c => Enumerable.Range(c > 0 ? 0 : 1, c < Width ? c + 1 : c)
                                                 .Select(r => Math.Sqrt(Math.Pow(c, 2) + Math.Pow(r, 2))))
                      .Distinct()
                      .OrderBy(l => l)
                      .ToList();
        }


        public class NodeDistance
        {
            public Node Node { get; set; }
            public double Distance { get; set; }

        }

        public class InterNodeDistance
        {
            public Node FromNode { get; set; }
            public Node ToNode { get; set; }
            public double Distance { get; set; }
        }
    }
}
