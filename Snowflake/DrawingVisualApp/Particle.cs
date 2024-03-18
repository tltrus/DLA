using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using VectorOperation;

namespace DrawingVisualApp
{
    class Particle
    {
        public Vector2D pos;
        double r;

        public Particle(double radius, double angle)
        {
            pos = Vector2D.FromAngle(angle);
            pos.Mult(radius);
            r = 2;
        }

        public void Update()
        {
            pos.x -= 1;
            pos.y += MainWindow.rnd.Next(-3, 4);

            var angle = pos.HeadingRad();
            angle = Constrain(angle, 0, Math.PI / 6);

            var magnitude = pos.Mag();
            pos = Vector2D.FromAngle(angle);
            pos.SetMag(magnitude);
        }

        public void Draw(DrawingContext dc, Brush color)
        {
            var x = pos.x + MainWindow.width / 2;
            var y = pos.y + MainWindow.height / 2;
            dc.DrawEllipse(color, null, new Point(x, y), r, r);
        }

        public bool isIntersects(List<Particle> snowflake)
        {
            bool result = false;
            foreach (var s in snowflake)
            {
                var d = Vector2D.Dist(s.pos, this.pos);
                if (d < this.r * 2)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public bool isFinished() => pos.x < 1;
        private double Constrain(double n, double low, double high) => Math.Max(Math.Min(n, high), low);
    }
}
