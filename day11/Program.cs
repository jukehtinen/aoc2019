using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace day11
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

    class Bot
    {
        private static readonly List<(int X, int Y)> Directions = new List<(int X, int Y)> { (0, -1), (1, 0), (0, 1), (-1, 0) };

        public (int X, int Y) Pos { get; private set; } = (0, 0);

        private int _currentDir;

        public void Step(int move)
        {
            if (move == 0)
            {
                _currentDir--;
                if (_currentDir == -1) _currentDir = 3;
            }
            else
            {
                _currentDir++;
                if (_currentDir == 4) _currentDir = 0;
            }
            Pos = (Pos.X + Directions[_currentDir].X, Pos.Y + Directions[_currentDir].Y);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var code = File.ReadAllText("input.txt").Split(',').Select(long.Parse).ToList();

            // Part 1
            var bot = new Bot();
            var map = new Dictionary<(int, int), int>();
            var output = new Stack<int>();
            var icc = new IntcodeComputer(code.ToList()) { Output = o => output.Push((int)o) };
            while (true)
            {
                icc.Run(new Stack<long>(new long[] { map.ContainsKey(bot.Pos) ? map[bot.Pos] : 0 }));
                if (icc.IsHalted)
                    break;

                var move = output.Pop();
                map[bot.Pos] = output.Pop();
                bot.Step(move);
            }
            Console.WriteLine(map.Count);

            // Part 2
            bot = new Bot();
            map = new Dictionary<(int, int), int> { [bot.Pos] = 1 };
            output = new Stack<int>();
            icc = new IntcodeComputer(code.ToList()) { Output = o => output.Push((int)o) };
            while (true)
            {
                icc.Run(new Stack<long>(new long[] { map.ContainsKey(bot.Pos) ? map[bot.Pos] : 0 }));
                if (icc.IsHalted)
                    break;

                var move = output.Pop();
                map[bot.Pos] = output.Pop();
                bot.Step(move);
            }

            for (var y = 0; y < map.Keys.Max(k => k.Item2) + 1; y++)
            {
                for (var x = 0; x < map.Keys.Max(k => k.Item1) + 1; x++)
                {
                    if (map.TryGetValue((x, y), out var val))
                        Console.Write(val == 1 ? "#" : " ");
                    else
                        Console.Write(" ");
                }
                Console.Write("\n");
            }
        }
    }
}
