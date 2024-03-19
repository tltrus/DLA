using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace DrawingVisualApp
{
    class Walker
    {
        public Vector2D pos;
        public SolidColorBrush brush;
        public double r;
        bool stuck;
         

        public Walker(SolidColorBrush brush)
        {
            this.brush = brush;
            pos = RandomPoint();
            r = MainWindow.radius;
        }
        public Walker(int x, int y, SolidColorBrush brush)
        {
            pos = new Vector2D(x, y);
            this.brush = brush;
            r = MainWindow.radius;
        }

        public void Walk()
        {
            var vel = new Vector2D().Random2D(MainWindow.rnd);
            vel = vel.Mult(5);
            pos.Add(vel);
            
            pos.x = Constrain(pos.x, 0, MainWindow.width);
            pos.y = Constrain(pos.y, 0, MainWindow.height);
        }

        public bool CheckStuck(List<Walker> others)
        {
            foreach (var i in others)
            {
                var dist = DistSq(pos, i.pos);
                if (dist < r * i.r * 4)
                {
                    stuck = true;
                    return true;
                    //break;
                }
            }
            return false;
        }


        private double Constrain(double n, double low, double high) => Math.Max(Math.Min(n, high), low);

        private double DistSq(Vector2D a, Vector2D b)
        {
            var dx = b.x - a.x;
            var dy = b.y - a.y;
            return dx * dx + dy * dy;
        }

        // Создание случайных точек в четырех краях поля
        private Vector2D RandomPoint()
        {
            int i = MainWindow.rnd.Next(0, 4); // от 0 до 3 (4 стороны направления)
            
            if (i == 0)
            {
                var x = MainWindow.rnd.Next(0, MainWindow.width);
                return new Vector2D(x, 0);
            }
            else if (i == 1)
            {
                var x = MainWindow.rnd.Next(0, MainWindow.width);
                return new Vector2D(x, MainWindow.height);
            }
            else if (i == 2)
            {
                var y = MainWindow.rnd.Next(0, MainWindow.height);
                return new Vector2D(0, y);
            }
            else
            {
                var y = MainWindow.rnd.Next(0, MainWindow.height);
                return new Vector2D(MainWindow.width, y);
            }
        }
    }
}
