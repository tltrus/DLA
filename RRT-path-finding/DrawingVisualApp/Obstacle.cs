using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DrawingVisualApp
{
    internal class Obstacle
    {
        public Vector2D pos;
        public double radius;

        public Obstacle(int radius = 20)
        {
            this.radius = radius;
        }

        public void Draw(DrawingContext dc)
        {
            Point p = new Point(pos.X, pos.Y);
            dc.DrawEllipse(Brushes.Gray, null, p, radius, radius);
        }
    }
}
