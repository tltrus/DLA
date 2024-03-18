using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Windows;
using System.Windows.Media;

namespace DrawingVisualApp
{

    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer Drawtimer;
        public static Random rnd = new Random();
        public static int width, height;

        DrawingVisual visual;
        DrawingContext dc;

        Particle current;
        Particle currentMirriow;
        List<Particle> snowflake = new List<Particle>();


        public MainWindow()
        {
            InitializeComponent();

            visual = new DrawingVisual();
            width = (int)g.Width;
            height = (int)g.Height;

            current = new Particle(width / 2, 0);
            currentMirriow = new Particle(width / 2, 0);

            Drawtimer = new System.Windows.Threading.DispatcherTimer();
            Drawtimer.Tick += new EventHandler(DrawtimerTick);
            Drawtimer.Interval = new TimeSpan(0, 0, 0, 0, 5);

            Drawtimer.Start();
        }

        private void DrawtimerTick(object sender, EventArgs e) => Drawing();

        private void Drawing()
        {
            g.RemoveVisual(visual);

            using (dc = visual.RenderOpen())
            {
                if (current.isIntersects(snowflake))
                {
                    Reset();
                }

                while (!current.isFinished() && !current.isIntersects(snowflake))
                {
                    current.Update();
                }

                snowflake.Add(current);
                current = new Particle(width / 2, 0);

                foreach (var p in snowflake)
                {
                    // Main particle
                    p.Draw(dc, Brushes.White);

                    // Mirrow of main particle -  y
                    Particle pmirr = new Particle(0, 0);
                    pmirr.pos.x = p.pos.x;
                    pmirr.pos.y = p.pos.y * -1;
                    pmirr.Draw(dc, Brushes.White);

                    // Copy and turning of rays
                    Particle pnew = new Particle(0, 0);
                    pnew.pos = p.pos.CopyToVector();
                    Particle pmirrnew = new Particle(0, 0);
                    pmirrnew.pos = pmirr.pos.CopyToVector();

                    for (int i = 0; i < 5; ++i)
                    {
                        pnew.pos.Rotate(Math.PI / 3);
                        pnew.Draw(dc, Brushes.White);

                        pmirrnew.pos.Rotate(Math.PI / 3);
                        pmirrnew.Draw(dc, Brushes.White);
                    }
                }
                dc.Close();
                g.AddVisual(visual);
            }
        }

        private void Reset()
        {
            current = new Particle(width / 2, 0);
            currentMirriow = new Particle(width / 2, 0);
            snowflake.Clear();
        }
    }
}
