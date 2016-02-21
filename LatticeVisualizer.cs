using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph;

namespace PSU.NocSym.Core
{
    public class LatticeVisualizer : IVertexAndEdgeListGraph<Node, Wire>
    {
        private SquareLattice Lattice { get; set; }
        public LatticeVisualizer(SquareLattice lattice)
        {
            Lattice = lattice;
        }

        public bool ContainsEdge(Node source, Node target)
        {
            return source.IsConnectedTo(target);
        }

        public bool TryGetEdge(Node source, Node target, out Wire edge)
        {
            edge = null;

            if (!source.IsConnectedTo(target))
            {                
                return false;
            }

            edge = source.Connections
                  .Where(c => c.OppositeEnd.Node == target)
                  .Select(c => c.Wire)
                  .FirstOrDefault();
            return edge != null;            
        }

        public bool TryGetEdges(Node source, Node target, out IEnumerable<Wire> edges)
        {
            edges = null;
            Wire edge = null;
            if (!TryGetEdge(source, target, out edge))
            {
                return false;
            }

            edges = new[] { edge }.AsEnumerable();
            return true;
        }

        public bool IsOutEdgesEmpty(Node v)
        {
            return v.Connections.Count == 0;
        }

        public int OutDegree(Node v)
        {
            return v.Connections.Count;
        }

        public Wire OutEdge(Node v, int index)
        {
            var con = v.Connections.ElementAtOrDefault(index);
            if (con != null)
            {
                return con.Wire;
            }

            return null;
        }

        public IEnumerable<Wire> OutEdges(Node v)
        {
            return v.Connections.Select(c => c.Wire).AsEnumerable();
        }

        public bool TryGetOutEdges(Node v, out IEnumerable<Wire> edges)
        {
            edges = OutEdges(v);
            return true;
        }

        public bool AllowParallelEdges
        {
            get { return false; }
        }

        public bool IsDirected
        {
            get { return true; }
        }

        public bool ContainsVertex(Node vertex)
        {
            return true;
        }

        public bool IsVerticesEmpty
        {
            get { return false; }
        }

        public int VertexCount
        {
            get { return Lattice.Nodes.Count; }
        }

        public IEnumerable<Node> Vertices
        {
            get { return Lattice.Nodes.AsEnumerable(); }
        }

        public bool ContainsEdge(Wire edge)
        {
            return true;
        }

        public int EdgeCount
        {
            get { return Edges.Count(); }
        }

        public IEnumerable<Wire> Edges
        {
            get 
            { 
                return Lattice.Nodes
                              .SelectMany(n => n.Connections.Select(c => c.Wire))
                              .Distinct()
                              .AsEnumerable();
            }
        }

        public bool IsEdgesEmpty
        {
            get { return false; }
        }
    }
}
