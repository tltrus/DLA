using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLA
{
    internal class Walker
    {
        public PointP pos;

        public Walker()
        {
            pos = new PointP();
        }

        public void SetRandomPosition(Random rnd, double radius, PointP seed)
        {
            double theta = 2 * Math.PI * rnd.NextDouble();      // random angle theta
            int x = (int)(radius * Math.Cos(theta)) + seed.x;
            int y = (int)(radius * Math.Sin(theta)) + seed.y;
            pos = new PointP(x, y);
        }

    }
}
