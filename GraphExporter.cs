﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using System.Drawing;

namespace PSU.NocSym.Core
{
    public class GraphExporter
    {
        Stack<Color> Colors { get; set; }
        SortedList<Node, Color> NodeToColor { get; set; }
        public GraphExporter()
        {
            NodeToColor = new SortedList<Node, Color>();
            Colors = new Stack<Color>(
            new[] {
                Color.Red,
                Color.Blue,
                Color.Green,
                Color.Orange,
                Color.Brown,
                Color.Yellow,
                Color.Pink,
                Color.Purple,
                Color.Teal,
                Color.Cyan,
                Color.Magenta,
                Color.Olive,
                Color.Orchid,
                Color.Sienna,
                Color.SlateGray,
                Color.Tomato,
                Color.Peru,
                Color.Lavender,
                Color.Goldenrod,
                Color.Gainsboro,
                Color.Lime,
                Color.MediumTurquoise,
                Color.PowderBlue
            });
        }

        public void Export(SquareLattice lattice, string path)
        {
            var v = new LatticeVisualizer(lattice);            
            var a = new GraphvizAlgorithm<Node, Wire>(v);
            
            a.GraphFormat.Size = new SizeF(10000f, 10000f);
            a.FormatVertex += a_FormatVertex;
            a.FormatEdge += a_FormatEdge;
            a.Generate(new DotEngine(), path);
        }

        void a_FormatEdge(object sender, FormatEdgeEventArgs<Node, Wire> e)
        {
            e.EdgeFormatter.Dir = QuickGraph.Graphviz.Dot.GraphvizEdgeDirection.None;
            e.EdgeFormatter.Style = QuickGraph.Graphviz.Dot.GraphvizEdgeStyle.Bold;         
        }

        void a_FormatVertex(object sender, FormatVertexEventArgs<Node> e)
        {
            e.VertexFormatter.Position = new Point((e.Vertex.Row + 1) * 2, (e.Vertex.Column + 1) * 2);
            e.VertexFormatter.Shape = QuickGraph.Graphviz.Dot.GraphvizVertexShape.Box;
            e.VertexFormatter.BottomLabel = string.Empty;
            e.VertexFormatter.Comment = string.Empty;
            e.VertexFormatter.Label = ((e.Vertex.Row + 1) * (e.Vertex.Column + 1)).ToString();
            e.VertexFormatter.Style = GraphvizVertexStyle.Filled;

            AssignNodeColor(e);
        }

        private void AssignNodeColor(FormatVertexEventArgs<Node> e)
        {
            Color color;
            if (!NodeToColor.TryGetValue(e.Vertex, out color))
            {
                var nodes = new List<Node>();
                GetGraphNodes(e.Vertex, nodes);
                color = Colors.Pop();

                nodes.ForEach(n => NodeToColor.Add(n, color));
            }

            e.VertexFormatter.FillColor = color;
        }

        private void GetGraphNodes(Node node, List<Node> knownNodes)
        {
            var newNodes = 
                node.Connections
                    .Select(c => c.OppositeEnd.Node)
                    .Distinct()
                    .Where(n => !knownNodes.Contains(n))
                    .ToList();
            knownNodes.AddRange(newNodes);
            newNodes.ForEach(n => GetGraphNodes(n, knownNodes));
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
