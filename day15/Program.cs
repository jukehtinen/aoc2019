using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace day15
{
    class IntcodeComputer
    {
        public enum AccessMode { Position, Immediate, Relative };
        public bool IsHalted { get; set; }
        public Action<long> Output { get; set; }

        private readonly Dictionary<int, long> _code = new Dictionary<int, long>();
        private int _ip;
        private int _relativeBase;

        public IntcodeComputer(IEnumerable<long> code)
        {
            var count = 0;
            foreach (var op in code)
            {
                _code[count++] = op;
            }
        }

        public long Get(AccessMode mode, int param)
        {
            try
            {
                switch (mode)
                {
                    case AccessMode.Position: return _code[(int)_code[param]];
                    case AccessMode.Immediate: return _code[param];
                    case AccessMode.Relative: return _code[_relativeBase + (int)_code[param]];
                }
            }
            catch (KeyNotFoundException)
            {
            }
            // Memory beyond the initial program starts with the value 0
            return 0;
        }

        public void Set(AccessMode mode, int param, long value)
        {
            switch (mode)
            {
                case AccessMode.Position: _code[(int)_code[param]] = value; break;
                case AccessMode.Relative: _code[_relativeBase + (int)_code[param]] = value; break;
            }
        }

        public void Run(Stack<long> inputs)
        {
            while (true)
            {
                var instruction = _code[_ip];
                var op = instruction % 100;
                var a0 = (AccessMode)(instruction / 100 % 10);
                var a1 = (AccessMode)(instruction / 1000 % 10);
                var a2 = (AccessMode)(instruction / 10000 % 10);

                // add
                if (op == 1)
                {
                    Set(a2, _ip + 3, Get(a0, _ip + 1) + Get(a1, _ip + 2));
                    _ip += 4;
                }
                // mul
                if (op == 2)
                {
                    Set(a2, _ip + 3, Get(a0, _ip + 1) * Get(a1, _ip + 2));
                    _ip += 4;
                }
                // input
                if (op == 3)
                {
                    if (inputs.Count == 0) break;
                    Set(a0, _ip + 1, inputs.Pop());
                    _ip += 2;
                }
                // output
                if (op == 4)
                {
                    Output?.Invoke(Get(a0, _ip + 1));
                    _ip += 2;
                }
                // jump-if-true
                if (op == 5)
                {
                    var p0 = Get(a0, _ip + 1);
                    var p1 = Get(a1, _ip + 2);
                    if (p0 != 0)
                        _ip = (int)p1;
                    else
                        _ip += 3;
                }
                // jump-if-false
                if (op == 6)
                {
                    var p0 = Get(a0, _ip + 1);
                    var p1 = Get(a1, _ip + 2);
                    if (p0 == 0)
                        _ip = (int)p1;
                    else
                        _ip += 3;
                }
                // less than
                if (op == 7)
                {
                    var p0 = Get(a0, _ip + 1);
                    var p1 = Get(a1, _ip + 2);
                    Set(a2, _ip + 3, p0 < p1 ? 1 : 0);
                    _ip += 4;
                }
                // equals
                if (op == 8)
                {
                    var p0 = Get(a0, _ip + 1);
                    var p1 = Get(a1, _ip + 2);
                    Set(a2, _ip + 3, p0 == p1 ? 1 : 0);
                    _ip += 4;
                }
                // adjusts the relative base
                if (op == 9)
                {
                    _relativeBase += (int)Get(a0, _ip + 1);
                    _ip += 2;
                }
                // halt
                if (op == 99)
                {
                    IsHalted = true;
                    break;
                }
            }
        }
    }

    class Maze
    {
        Dictionary<(int, int), int> maze = new Dictionary<(int, int), int>();
        IntcodeComputer icc;
        int output;

        (int x, int y) oxygenSystemLocation = (x: 0, y: 0);
        int maxtime;

        public void Solve()
        {
            icc = new IntcodeComputer(File.ReadAllText("input.txt").Split(',').Select(long.Parse).ToList());
            icc.Output = o => output = (int)o;

            // Part 1
            Solve(0, -1, 1, 1);
            Console.WriteLine(maze[oxygenSystemLocation]);

            // Print();

            // Part 2
            Solve2(oxygenSystemLocation.x, oxygenSystemLocation.y, 0);
            Console.WriteLine(maxtime);
        }

        // Flood fill and step back
        bool Solve(int x, int y, int move, int stepsFromStart)
        {
            if (maze.ContainsKey((x, y)))
                return false;

            icc.Run(new Stack<long>(new long[] { move }));
            if (output == 0)
            {
                maze[(x, y)] = -1;
                return false;
            }
            if (output == 2)
                oxygenSystemLocation = (x, y);

            maze[(x, y)] = stepsFromStart;

            if (Solve(x + 1, y, 2, stepsFromStart + 1))
                icc.Run(new Stack<long>(new long[] { 1 }));
            if (Solve(x - 1, y, 1, stepsFromStart + 1))
                icc.Run(new Stack<long>(new long[] { 2 }));
            if (Solve(x, y + 1, 4, stepsFromStart + 1))
                icc.Run(new Stack<long>(new long[] { 3 }));
            if (Solve(x, y - 1, 3, stepsFromStart + 1))
                icc.Run(new Stack<long>(new long[] { 4 }));

            return true;
        }

        // Just flood fill
        void Solve2(int x, int y, int time)
        {
            if (maze[(x, y)] == -1 || maze[(x, y)] == 9999)
                return;

            maze[(x, y)] = 9999;

            maxtime = Math.Max(maxtime, time);

            Solve2(x + 1, y, time + 1);
            Solve2(x - 1, y, time + 1);
            Solve2(x, y + 1, time + 1);
            Solve2(x, y - 1, time + 1);
        }

        void Print()
        {
            for (var y = 0; y < 80; y++)
            {
                for (var x = 0; x < 80; x++)
                {
                    if (maze.ContainsKey((x - 30, y - 30)))
                    {
                        var tile = maze[(x - 30, y - 30)];
                        if (tile == -1)
                            Console.Write("#");
                        else
                            Console.Write(".");
                    }
                    else
                        Console.Write("?");
                }
                Console.Write("\n");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            new Maze().Solve();
        }
    }
}
