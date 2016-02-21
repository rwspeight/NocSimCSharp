using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PSU.NocSym.Core
{
    
        //~ Build network grid
        //~ For requested number of wires
            //~ Pull wire from distribution
            //~ Choose random wire angle
            //~ Choose random grid position
            //~ Record the wire specs, the grid position, and orientation 

        /* Interesting thoughts
         * 
         * - The problem of discrete lattice points
         *      If I define a lattice having discrete distances, basically
         *      ordered pairs (1,2), (1,3), etc... I have the problem that
         *      angled wires will fall between the integral positions.  A
         *      complex way out of this is to define hit boxes for each
         *      lattice point.  This would require extra computation to
         *      determine a connection.  Regardless this effectively requires
         *      continuous distances to be defined.
         * 
         * 
         */       
    

    public abstract class AbstractLattice
    {
        //& Think about changing the double to be some generic type (like a Unit or a Metric).  Or just make it generic :-P
        public double Width { get; set; }
        public double Height { get; set; }


        public AbstractLattice(double width, double height)
        {
            Width = width;
            Height = height;
        }
        
        public Node GetNodeAt(double x, double y)
        {
            return null;
        }
    }

    public class HitboxTracker
    {
        private SortedList<double, SortedList<double, SortedList<double, HashSet<double>>>> Index = new SortedList<double, SortedList<double, SortedList<double, HashSet<double>>>>();        

        public void Add(double x1, double y1, double x2, double y2)
        {
            SortedList<double, SortedList<double, HashSet<double>>> second = null;
            SortedList<double, HashSet<double>> third = null;
            HashSet<double> fourth = null;

            if (!Index.TryGetValue(x1, out second))
            {
                second = new SortedList<double, SortedList<double, HashSet<double>>>();
                Index.Add(x1, second);
            }

            if (!second.TryGetValue(y1, out third))
            {
                third = new SortedList<double, HashSet<double>>();
                second.Add(y1, third);
            }

            if (!third.TryGetValue(x1, out fourth))
            {
                fourth = new HashSet<double>();
                third.Add(x2, fourth);
            }

            fourth.Add(y2);
        }

        public bool IsHit(double x1, double y1, double x2, double y2)
        {
            var second = Index.Where(k => k.Key <= x1).Select(k => k.Value).LastOrDefault();
            if (second == null) { return false; }

            var third = second.Where(k => k.Key <= y1).Select(k => k.Value).LastOrDefault();
            if (third == null) { return false; }

            var fourth = third.Where(k => k.Key <= x1).Select(k => k.Value).FirstOrDefault();
            if (fourth == null) { return false; }

            //return .Where(k => k.Key <= x1).Select(k => k.Value).FirstOrDefault();
            return false;

        }
    }

    
}
