using System;

namespace DrawingVisualApp
{
    internal class Vector2D
    {
        public double x { get; set; }

        public double y { get; set; }

        public Vector2D(double x = 0.0, double y = 0.0)
        {
            this.x = x;
            this.y = y;
        }

        public void Add(Vector2D Vector2D)
        {
            x += Vector2D.x;
            y += Vector2D.y;
        }

        public static Vector2D Add(Vector2D v1, Vector2D v2)
        {
            Vector2D Vector2D = new Vector2D();
            Vector2D.x = v1.x + v2.x;
            Vector2D.y = v1.y + v2.y;
            return Vector2D;
        }

        public void Sub(Vector2D Vector2D)
        {
            x -= Vector2D.x;
            y -= Vector2D.y;
        }

        public static Vector2D Sub(Vector2D v1, Vector2D v2)
        {
            Vector2D Vector2D = new Vector2D();
            Vector2D.x = v1.x - v2.x;
            Vector2D.y = v1.y - v2.y;
            return Vector2D;
        }

        public Vector2D Div(double val)
        {
            x /= val;
            y /= val;
            return this;
        }

        public Vector2D Mult(double val)
        {
            x *= val;
            y *= val;
            return this;
        }

        public static Vector2D Mult(Vector2D v1, Vector2D v2)
        {
            Vector2D Vector2D = new Vector2D();
            Vector2D.x = v1.x * v2.x;
            Vector2D.y = v1.y * v2.y;
            return Vector2D;
        }

        public double MagSq()
        {
            double num = x;
            double num2 = y;
            return num * num + num2 * num2;
        }

        public Vector2D Limit(double max)
        {
            double num = MagSq();
            if (num > max * max)
            {
                Div(Math.Sqrt(num)).Mult(max);
            }

            return this;
        }

        public double Mag()
        {
            return Math.Sqrt(MagSq());
        }

        public Vector2D Normalize()
        {
            double num = Mag();
            if (num != 0.0)
            {
                Mult(1.0 / num);
            }

            return this;
        }

        public Vector2D SetMag(int n)
        {
            return Normalize().Mult(n);
        }

        public Vector2D SetMag(double n)
        {
            return Normalize().Mult(n);
        }

        private Vector2D FromAngle(double angle, double length = 1.0)
        {
            return new Vector2D(length * Math.Cos(angle), length * Math.Sin(angle));
        }

        public Vector2D Random2D(Random rnd)
        {
            return FromAngle(rnd.NextDouble() * Math.PI * 2.0);
        }
    }
}
