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

    class Program
    {
        static void Main(string[] args)
        {
            var code = File.ReadAllText("input.txt").Split(',').Select(long.Parse).ToList();

            // Part 1
            var blocks = 0;
            var count = 0;
            var icc = new IntcodeComputer(code.ToList())
            {
                Output = o =>
                {
                    count++;
                    if (count % 3 == 0 && o == 2)
                        blocks++;
                }
            };
            while (true)
            {
                icc.Run(new Stack<long>(new long[] { }));
                if (icc.IsHalted)
                    break;
            }
            Console.WriteLine(blocks);


            // Part 2
            var score = 0;
            var src = code.ToList();
            var ballX = 0;
            var paddleX = 0;
            src[0] = 2;

            var output = new Stack<int>();
            icc = new IntcodeComputer(src)
            {
                Output = o =>
                {
                    output.Push((int)o);
                    if (output.Count == 3)
                    {
                        var tile = output.Pop();
                        var y = output.Pop();
                        var x = output.Pop();

                        if (x != -1)
                        {
                            /*
                            Console.SetCursorPosition(x, y);
                            if (tile == 0) Console.Write(" ");
                            if (tile == 1) Console.Write("#");
                            if (tile == 2) Console.Write("\"");
                            if (tile == 3) Console.Write("=");
                            if (tile == 4) Console.Write("*");
                            */

                            if (tile == 3)
                                paddleX = x;

                            if (tile == 4)
                                ballX = x;
                        }
                        else
                        {
                            score = tile;
                        }
                    }
                }
            };

            while (true)
            {
                long input = 0;
                if (paddleX < ballX)
                    input = 1;
                if (paddleX > ballX)
                    input = -1;

                icc.Run(new Stack<long>(new long[] { input }));
                if (icc.IsHalted)
                    break;
            }

            Console.WriteLine(score);
        }
    }
}
