using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace DLA
{
    struct PointP
    {
        public int x, y;
        public PointP(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    struct Map
    {
        public int state;
        public int dist;
    }

    public partial class MainWindow : Window
    {
        Random rnd = new Random();
        WriteableBitmap wb;
        int width, height, cols, rows;
        System.Windows.Threading.DispatcherTimer timer;
        int steps = 0;     // steps conter
        bool foundFriend = false;    // found another particle
        bool exitCircle = false;     // reached the required radius
        bool nearEdge = false;       // near the edge of the field
        double radius = 5;
        List<Walker> walkers;
        PointP seed;
        PointP edgePoint;

        int cellSize = 2;
        Map[,] map;

        public MainWindow()
        {
            InitializeComponent();

            width = (int)img.Width; height = (int)img.Height;
            cols = width / cellSize; rows = height / cellSize;

            wb = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null); img.Source = wb;

            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(TimerTick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);

            map = new Map[rows, cols];
            seed = new PointP(cols / 2, rows / 2);
            map[rows / 2, cols / 2].state = 1;

            edgePoint = seed;
            radius = SetRadius(seed, radius);

            Init();

            Drawing();
        }

        private void Init()
        {
            // Map initialization
            int centerX = cols / 2;
            int centerY = rows / 2;
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int dist = (int)Math.Sqrt((centerX - x) * (centerX - x) + (centerY - y) * (centerY - y));
                    map[y, x].dist = dist;
                }
            }

                // Add walkers
            walkers = new List<Walker>();
            for (int i = 0; i < 40; i++)
            {
                Walker walker = new Walker();
                walker.SetRandomPosition(rnd, radius, seed);
                walkers.Add(walker);
                map[walker.pos.y, walker.pos.x].state = 3;
            }
        }


        private void Walk()
        {
            steps++;

            for (int i = 0; i < walkers.Count; i++)
            {
                PointP newwalker = CheckAround(walkers[i].pos);

                // Walker close to another particle
                if (foundFriend)
                {
                    map[walkers[i].pos.y, walkers[i].pos.x].state = 1;
                    radius = SetRadius(walkers[i].pos, radius);

                    walkers[i].SetRandomPosition(rnd, radius, seed);
                }
                else
                if (!foundFriend && !nearEdge && !exitCircle)
                {
                    walkers[i].pos = newwalker;
                    map[newwalker.y, newwalker.x].state = 3;
                }
                else
                {
                    // Генерируем случайное положение частицы
                    walkers[i].SetRandomPosition(rnd, radius, seed);
                }
            }

            Drawing();

            if (steps >= 400000) timer.Stop();
        }

        private PointP CheckAround(PointP walker)
        {
            foundFriend = false;    // found another particle
            exitCircle = false;     //reached the required radius
	        nearEdge = false;       //near the edge of the field
            PointP newwalker = walker;
            
            // Проверка: если частица около края
            if ((walker.x + 1) >= cols - 1 || (walker.x - 1) < 1 || 
                (walker.y + 1) >= rows - 1 || (walker.y - 1) < 1)
		            nearEdge = true;

            if (!nearEdge)
            {
                double dist = Math.Sqrt((seed.x - walker.x) * (seed.x - walker.x) + (seed.y - walker.y) * (seed.y - walker.y));
                if (dist >= radius)
                    exitCircle = true;

                int neighborDown = map[walker.y + 1, walker.x].state;
                if (neighborDown == 1)
                    foundFriend = true;

                int neighborUp = map[walker.y - 1, walker.x].state;
                if (neighborUp == 1)
			        foundFriend = true;

                int neighborRight = map[walker.y, walker.x + 1].state;
                if (neighborRight == 1)
			        foundFriend = true;

                int neighborLeft = map[walker.y, walker.x - 1].state;
                if (neighborLeft == 1)
			        foundFriend = true;
            }

            // После проверки положения, если положение ОК, то запускаем случайный ход
            if (!foundFriend && !nearEdge)
            {
                double decide = rnd.NextDouble();
                if (decide < 0.25)
                    newwalker = new PointP(walker.x - 1, walker.y);
                else
                if (decide < 0.5)
                    newwalker = new PointP(walker.x + 1, walker.y);
                else
                if (decide < 0.75)
                    newwalker = new PointP(walker.x, walker.y + 1);
                else
                    newwalker = new PointP(walker.x, walker.y - 1);
            }

            return newwalker;
        }
        private double SetRadius(PointP p, double radius)
        {
            //// without sqrt to speed up
            double dist_walker = (seed.x - p.x) * (seed.x - p.x) + (seed.y - p.y) * (seed.y - p.y);
            //double dist_edge = (seed.x - edgePoint.x) * (seed.x - edgePoint.x) + (seed.y - edgePoint.y) * (seed.y - edgePoint.y);

            //if (dist_walker - dist_edge > 8) // it's a bad solution. Should be improve.
            //{
            //    edgePoint = p;
            //    return radius + 1;
            //}
            //return radius;
            var farPoint = GetFarEdgePoint(map);
            //double dist_edge = (seed.x - farPoint.x) * (seed.x - farPoint.x) + (seed.y - farPoint.y) * (seed.y - farPoint.y);
            if (radius - farPoint <= 2)
            {
                //edgePoint = farPoint;
                return radius + 1;
            }
            return radius;
        }

        private int GetFarEdgePoint(Map[,] map)
        {
            PointP maxPoint = new PointP();
            int dist_edge = 0;

            for (int y = 0; y < rows; ++y)
                for (int x = 0; x < cols; ++x)
                {
                    if (map[y, x].state == 1 && map[y, x].dist > dist_edge)
                    {
                        dist_edge = map[y, x].dist;
                        maxPoint.x = x;
                        maxPoint.y = y;
                    }
                }

            return dist_edge;
        }

        private void Drawing()
        {
            for (int y = 0; y < rows; ++y)
                for (int x = 0; x < cols; ++x)
                {
                    int newx = x * cellSize;
                    int newy = y * cellSize;

                    if (map[y, x].state == 1)
                        wb.FillRectangle(newx, newy, newx + cellSize, newy + cellSize, Colors.Black);
                    if (map[y, x].state == 2)
                        wb.FillRectangle(newx, newy, newx + cellSize, newy + cellSize, Colors.LightGray);
                    if (map[y, x].state == 3)
                    {
                        wb.FillRectangle(newx, newy, newx + cellSize, newy + cellSize, Colors.Red);
                        map[y, x].state = 0;
                    }

                    double dist = (int)(Math.Sqrt((seed.x - x) * (seed.x - x) + (seed.y - y) * (seed.y - y)));
                    if (dist == (int)radius)
                        wb.FillRectangle(newx, newy, newx + cellSize, newy + cellSize, Colors.LightGray);
                }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            wb.Clear();
            Walk();
        }
        private void Button_Click(object sender, RoutedEventArgs e) => timer.Start();
    }
}
