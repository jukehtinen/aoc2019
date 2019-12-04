using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace day03
{
    struct Point
    {
        public int X;
        public int Y;
    }

    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("input.txt");

            // Part 1
            var n1 = GetNodes(lines[0]);
            var n2 = GetNodes(lines[1]);
            n1.IntersectWith(n2);
            Console.WriteLine(n1.Min(n => Math.Abs(n.Item1) + Math.Abs(n.Item2)));

            // Part 2
            var n3 = GetNodes2(lines[0]);
            var n4 = GetNodes2(lines[1]);
            var intersect = n3.Keys.Intersect(n4.Keys).ToDictionary(t => t, t => n3[t] + n4[t]);
            Console.WriteLine(intersect.Min(v => v.Value));
        }

        private static HashSet<Tuple<int, int>> GetNodes(string line)
        {
            var set = new HashSet<Tuple<int, int>>();
            var dir = new Point();
            var current = new Point();
            foreach (var t in line.Split(','))
            {
                var val = int.Parse(t.Substring(1));
                if (t[0] == 'L') { dir.X = -1; dir.Y = 0; }
                if (t[0] == 'R') { dir.X = 1; dir.Y = 0; }
                if (t[0] == 'U') { dir.X = 0; dir.Y = -1; }
                if (t[0] == 'D') { dir.X = 0; dir.Y = 1; }

                for (var i = 0; i < val; i++)
                {
                    current.X += dir.X;
                    current.Y += dir.Y;
                    set.Add(new Tuple<int, int>(current.X, current.Y));
                }
            }

            return set;
        }

        private static Dictionary<Tuple<int, int>, int> GetNodes2(string line)
        {
            var set = new Dictionary<Tuple<int, int>, int>();
            var dir = new Point();
            var current = new Point();
            var steps = 0;
            foreach (var t in line.Split(','))
            {
                var val = int.Parse(t.Substring(1));
                if (t[0] == 'L') { dir.X = -1; dir.Y = 0; }
                if (t[0] == 'R') { dir.X = 1; dir.Y = 0; }
                if (t[0] == 'U') { dir.X = 0; dir.Y = -1; }
                if (t[0] == 'D') { dir.X = 0; dir.Y = 1; }

                for (var i = 0; i < val; i++)
                {
                    current.X += dir.X;
                    current.Y += dir.Y;
                    set.TryAdd(new Tuple<int, int>(current.X, current.Y), ++steps);
                }
            }

            return set;
        }
    }
}
