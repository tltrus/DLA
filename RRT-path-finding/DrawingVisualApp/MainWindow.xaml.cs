using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;



namespace DrawingVisualApp
{
    // Based on https://www.programmersought.com/article/34908226551/
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer timer;
        public static Random rnd = new Random();
        public static int width, height;

        DrawingVisual visual;
        DrawingContext dc;

        RRT rrt;
        Vector2D start, goal;
        List<Obstacle> obstacle_list;

        public MainWindow()
        {
            InitializeComponent();

            width = (int)g.Width;
            height = (int)g.Height;

            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 400);

            visual = new DrawingVisual();

            Init();
            timer.Start();
        }

        private void timerTick(object sender, EventArgs e) => Drawing();

        private void Init()
        {
            start = new Vector2D(rnd.Next(30,200), rnd.Next(30, 200));
            goal = new Vector2D(width - 100, height - 100);

            // Add obstacle
            obstacle_list = new List<Obstacle>();
            Obstacle obstacle = new Obstacle(70);
            obstacle.pos = new Vector2D(width / 2, height / 2);
            obstacle_list.Add(obstacle);
            obstacle = new Obstacle(40);
            obstacle.pos = new Vector2D(width / 1.5, height / 4);
            obstacle_list.Add(obstacle);
            obstacle = new Obstacle(20);
            obstacle.pos = new Vector2D(width / 4, height / 3);
            obstacle_list.Add(obstacle);

            // RRT
            rrt = new RRT(width, height);
            rrt.RRT_planning(start, goal, obstacle_list);
        }

        private void Drawing()
        {
            g.RemoveVisual(visual);

            List<Node> path_list = null;

            using (dc = visual.RenderOpen())
            {
                path_list = rrt.RRT_calculation();
                rrt.Draw(dc);

                // Draw start
                Point point = new Point(start.X, start.Y);
                dc.DrawEllipse(Brushes.LightBlue, null, point, 5, 5);

                // Draw goal
                point = new Point(goal.X, goal.Y);
                dc.DrawEllipse(Brushes.White, null, point, 15, 15);

                // Draw obstacle
                foreach (var obstacle in obstacle_list)
                {
                    obstacle.Draw(dc);
                }

                // Draw final path
                if (path_list != null)
                {
                    for (int i = 0; i < path_list.Count - 1; ++i)
                    { 
                        Point p0 = new Point(path_list[i].pos.X, path_list[i].pos.Y);           // current
                        Point p1 = new Point(path_list[i + 1].pos.X, path_list[i + 1].pos.Y);   // next
                        dc.DrawLine(new Pen(Brushes.LightGreen, 3), p0, p1);
                    }
                }

                dc.Close();
                g.AddVisual(visual);
            }

            if (path_list != null) timer.Stop();
        }

        private void g_MouseDown(object sender, MouseButtonEventArgs e)
        {
            timer.Stop();
            Init();
            timer.Start();
        }
    }
}
