using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace DrawingVisualApp
{
    internal class RRT
    {
        Random rnd  = new Random();
        int width, height;
        double expand_dis = 30.0;
        Node start, goal;
        List<Node> node_list = new List<Node>();
        List<Obstacle> obstacle_list = new List<Obstacle>();

        public RRT(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public void RRT_planning(Vector2D start, Vector2D goal, List<Obstacle> obstacle_list)
        {
            this.start = new Node(start.CopyToVector());
            this.goal = new Node(goal.CopyToVector());
            this.obstacle_list = obstacle_list;

            node_list.Add(this.start);
        }

        public List<Node> RRT_calculation()
        {
            // 1. Random sampling point in the environment
            Vector2D random_point = Get_random_point();

            // 2. Find the nearest node of distance sampling point in the node tree
            int n_ind = Get_nearest_list_index(node_list, random_point);
            Node nearest_node = node_list[n_ind];

            // 3. Grow a step in the direction of the sampling point to get the node of the next
            var new_node = Get_new_node(n_ind, nearest_node, random_point);

            // 4. Detect the collision, detect if the path to the newly generated node will collide with the obstacle
            bool no_collision = isSegment_collision(new_node);

            if (no_collision)
            {
                node_list.Add(new_node);

                if (isNear_goal(new_node))
                {
                    int last_index = node_list.Count - 1;
                    List<Node> path = Get_final_course(last_index);


                    return path;
                }
            }

            return null;
        }

        private List<Node> Get_final_course(int last_index)
        {
            List<Node> path = new List<Node>();

            path.Add(goal);

            while (node_list[last_index].cost != 0)
            {
                Node node = new Node();
                node.pos = node_list[last_index].pos;
                node.cost = node_list[last_index].cost;
                node.parent = node_list[last_index].parent;

                path.Add(node);
                last_index = node.parent;
            }

            path.Add(start);
            return path;
        }

        private bool isNear_goal(Node node)
        {
            var d = Line_cost(node, goal);
            if (d < expand_dis)
                return true;
            else
                return false;
        }

        private double Line_cost(Node node1, Node node2) => Math.Sqrt((node1.pos.X - node2.pos.X) * (node1.pos.X - node2.pos.X) + (node1.pos.Y - node2.pos.Y) * (node1.pos.Y - node2.pos.Y));

        private bool isSegment_collision(Node node)
        {
            foreach (var o in obstacle_list)
            {
                var dx = o.pos.X - node.pos.X;
                var dy = o.pos.Y - node.pos.Y;
                var d = Math.Sqrt(dx * dx + dy * dy);

                if (d <= o.radius)
                    return false;  // collision
            }
            return true;
        }

        private Node Get_new_node(int n_ind, Node nearest_node, Vector2D sample_point)
        {
            Node new_node = new Node();
            new_node.pos = new Vector2D(nearest_node.pos.X, nearest_node.pos.Y);

            Vector2D direction = Vector2D.Sub(sample_point, nearest_node.pos);
            direction.Normalize();
            direction.Mult(expand_dis);

            new_node.pos.Add(direction);
            new_node.cost = nearest_node.cost + 1;
            new_node.parent = n_ind;
            return new_node;
        }

        private int Get_nearest_list_index(List<Node> node_list, Vector2D sample_point)
        {
            double d = double.MaxValue;
            Node nearest_node = new Node();

            foreach (var n in node_list)
            {
                double dist = Vector2D.Dist(sample_point, n.pos);
                if (dist < d)
                {
                    d = dist;
                    nearest_node = n;
                }
            }
            
            var indx = node_list.IndexOf(nearest_node); // Можно улучшить?
            return indx;
        }

        private Vector2D Get_random_point()
        {
            var x = rnd.Next(0, width);
            var y = rnd.Next(0, height);

            return new Vector2D(x, y);
        }

        public void Draw(DrawingContext dc)
        {
            foreach(var node in node_list)
            {
                node.Drawing(dc, node_list);
            }
        }
    }
}
