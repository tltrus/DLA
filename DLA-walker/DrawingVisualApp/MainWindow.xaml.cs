using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;


namespace DrawingVisualApp
{

    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer Drawtimer, Ctrltimer;
        public static Random rnd = new Random();
        public static int width, height;
        public static double radius = 16;

        DrawingVisual visual;
        DrawingContext dc;

        List<Walker> tree = new List<Walker>();
        List<Walker> walkers = new List<Walker>();
        int maxWalkers = 100;


        public MainWindow()
        {
            InitializeComponent();

            visual = new DrawingVisual();
            width = (int)window.Width;
            height = (int)window.Height;

            tree.Add(new Walker(width / 2, height / 2, new SolidColorBrush(Colors.DeepPink)));
            for (int i = 0; i < maxWalkers; ++i)
            {
                radius *= 0.99;
                var color = Color.FromRgb((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256));
                var brush = new SolidColorBrush(color);
                walkers.Add(new Walker(brush));
            }

            Drawtimer = new System.Windows.Threading.DispatcherTimer();
            Drawtimer.Tick += new EventHandler(DrawtimerTick);
            Drawtimer.Interval = new TimeSpan(0, 0, 0, 0, 10);

            Drawtimer.Start();

            Ctrltimer = new System.Windows.Threading.DispatcherTimer();
            Ctrltimer.Tick += new EventHandler(CtrltimerTick);
            Ctrltimer.Interval = new TimeSpan(0, 0, 0, 0, 1);

            Ctrltimer.Start();
        }

        private void DrawtimerTick(object sender, EventArgs e) => Drawing();
        private void CtrltimerTick(object sender, EventArgs e) => Control();

        private void Control()
        {
            foreach (var i in walkers.ToList())
            {
                i.Walk();
                if (i.CheckStuck(tree))
                {
                    i.brush = new SolidColorBrush(Colors.DeepPink);
                    tree.Add(i);
                    walkers.RemoveAt(walkers.IndexOf(i));
                }
            }

            while (walkers.Count < maxWalkers)
            {
                if (radius < 0.2) break; // Чтобы радиус не уходил в бесконечность близкую к нулю

                radius *= 0.99;
                if (radius > 1)
                    walkers.Add(new Walker(new SolidColorBrush(Colors.White)));
                if (radius < 0.2)
                    break;
            }
        }

        private void Drawing()
        {
            g.RemoveVisual(visual);

            using (dc = visual.RenderOpen())
            {
                foreach (var i in tree)
                {
                    dc.DrawEllipse(i.brush, null, new Point(i.pos.x, i.pos.y), i.r, i.r);
                }

                foreach (var i in walkers)
                {
                    dc.DrawEllipse(i.brush, null, new Point(i.pos.x, i.pos.y), i.r, i.r);
                }

                dc.Close();
                g.AddVisual(visual);
            }
        }

    }
}
