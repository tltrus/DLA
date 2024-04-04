using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace DrawingVisualApp
{
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer timer;
        public static Random rnd = new Random();
        public static int width, height;

        DrawingVisual visual;
        DrawingContext dc;
        Vector2D startPoint = new Vector2D();
        List<Vector2D> nodeList;
        int delta = 3;
        bool isClearing;


        public MainWindow()
        {
            InitializeComponent();

            width = (int)g.Width;
            height = (int)g.Height;

            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);

            Setup();
            timer.Start();
        }

        private void timerTick(object sender, EventArgs e) => Drawing();

        private void Setup()
        {
            nodeList = new List<Vector2D>();
            startPoint = new Vector2D(width / 2, height / 2);
            nodeList.Add(startPoint);
        }

        private void Drawing()
        {
            visual = new DrawingVisual(); // allows to put the particles

            using (dc = visual.RenderOpen())
            {
                if (!isClearing)
                {
                    // Draw tree
                    var x = rnd.Next(width);
                    var y = rnd.Next(height);
                    Vector2D q_rand = new Vector2D(x, y);
                    Vector2D q_near = Nearest_Vertex(q_rand, nodeList);
                    Vector2D q_new = New_Conf(q_near, q_rand, delta);
                    nodeList.Add(q_new);

                    var p = new Point(q_new.X, q_new.Y);
                    var p_old = new Point(q_near.X, q_near.Y);

                    dc.DrawLine(new Pen(Brushes.White, 0.6), p_old, p);
                }
                else
                {
                    // Draw black screen
                    Rect rect = new Rect()
                    {
                        X = 0,
                        Y = 0,
                        Width = width,
                        Height = height
                    };
                    dc.DrawRectangle(Brushes.Black, null, rect);
                }


                dc.Close();
                g.AddVisual(visual);
            }

            isClearing = false;
        }

        private Vector2D Nearest_Vertex(Vector2D q, List<Vector2D> nodeList)
        {
            double d = double.MaxValue;
            Vector2D nearest = new Vector2D();

            foreach (var n in nodeList)
            {
                double dist = Vector2D.Dist(q, n);
                if (dist < d)
                {
                    d = dist;
                    nearest = n.CopyToVector();
                }
            }
            return nearest;
        }
        private Vector2D New_Conf(Vector2D q_near, Vector2D q_rand, int delta)
        {
            Vector2D result = q_near.CopyToVector();
            Vector2D direction = Vector2D.Sub(q_rand, q_near);
            direction.Normalize();
            direction.Mult(delta);
            result.Add(direction);
            return result;
        }
        private void g_MouseDown(object sender, MouseButtonEventArgs e)
        {
            timer.Stop();
            Setup();
            isClearing = true;
            timer.Start();
        }
    }
}
