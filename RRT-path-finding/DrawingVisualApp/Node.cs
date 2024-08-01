using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DrawingVisualApp
{
    internal class Node
    {
        public Vector2D pos;
        public double cost;
        public int parent;

        public Node()
        {
            pos = new Vector2D();
        }

        public Node(Vector2D pos)
        {
            cost = 0;
            parent = 0;
            this.pos = pos;
        }

        public void Drawing(DrawingContext dc, List<Node> node_list, double pen_thickness = 0.5)
        {
            Point p0 = new Point(pos.X, pos.Y);

            var parent_node = node_list[parent];
            Point p1 = new Point(parent_node.pos.X, parent_node.pos.Y);
            dc.DrawLine(new Pen(Brushes.White, pen_thickness), p0, p1);
        }
    }
}
