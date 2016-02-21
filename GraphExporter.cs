using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph.Graphviz;

namespace PSU.NocSym.Core
{
    public class GraphExporter
    {
        public GraphExporter()
        {
        }

        public void Export(SquareLattice lattice, string path)
        {
            var v = new LatticeVisualizer(lattice);            
            var a = new GraphvizAlgorithm<Node, Wire>(v);

            a.GraphFormat.Size = new QuickGraph.Graphviz.Dot.GraphvizSizeF(10000f, 10000f);
            a.FormatVertex += a_FormatVertex;
            a.FormatEdge += a_FormatEdge;
            a.Generate(new DotEngine(), path);
        }

        void a_FormatEdge(object sender, FormatEdgeEventArgs<Node, Wire> e)
        {
            e.EdgeFormatter.Dir = QuickGraph.Graphviz.Dot.GraphvizEdgeDirection.None;
            e.EdgeFormatter.Style = QuickGraph.Graphviz.Dot.GraphvizEdgeStyle.Bold;
            e.EdgeFormatter.IsDecorated = true;
            e.EdgeFormatter.IsConstrained = true;
            
            
        }

        void a_FormatVertex(object sender, FormatVertexEventArgs<Node> e)
        {
            e.VertexFormatter.Position = new QuickGraph.Graphviz.Dot.GraphvizPoint((e.Vertex.Row + 1) * 2, (e.Vertex.Column + 1) * 2);
            e.VertexFormatter.Shape = QuickGraph.Graphviz.Dot.GraphvizVertexShape.Box;
            e.VertexFormatter.BottomLabel = string.Empty;
            e.VertexFormatter.Comment = string.Empty;
            e.VertexFormatter.Label = ((e.Vertex.Row + 1) * (e.Vertex.Column + 1)).ToString();
        }       

        private class DotEngine : IDotEngine
        {
            public string Run(QuickGraph.Graphviz.Dot.GraphvizImageType imageType, string dot, string outputFileName)
            {
                System.IO.File.WriteAllText(outputFileName, dot);
                return dot;
            }
        }
    }
}
