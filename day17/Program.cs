using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace day17
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
            var icc = new IntcodeComputer(File.ReadAllText("input.txt").Split(',').Select(long.Parse).ToList());
            var map = new List<int>();
            icc.Output = o => map.Add((int)o);

            // Part 1
            icc.Run(new Stack<long>(new long[] { }));
            map.ForEach(c => Console.Write((char)c));
            var width = map.IndexOf(10) + 1;
            var height = map.Count / width;
            var total = 0;
            for (var i = 0; i < map.Count; i++)
            {
                var p = (x: i % width, y: i / width);
                if (p.x > 0 && p.y > 0 && p.x < width - 1 && p.y < height - 1 &&
                    map[i] == 35 && map[i - 1] == 35 && map[i + 1] == 35 && map[i + width] == 35 && map[i - width] == 35)
                {
                    total += (p.x * p.y);
                }
            }
            Console.WriteLine(total);

            // Part 2
            // Find out the full path
            var pos = map.IndexOf('^');
            var dir = 0;
            var code = string.Empty;
            var steps = -1;
            while (true)
            {
                // move
                steps++;
                if (dir == 0 && pos - width > 0 && map[pos - width] == '#') { pos -= width; continue; }
                if (dir == 1 && map[pos + 1] == '#') { pos++; continue; }
                if (dir == 2 && pos + width < map.Count && map[pos + width] == '#') { pos += width; continue; }
                if (dir == 3 && map[pos - 1] == '#') { pos--; continue; }

                if (steps > 0) code += steps + ",";
                steps = -1;
                // turn
                if (dir == 0)
                {
                    if (map[pos - 1] == '#') { dir = 3; code += "L,"; }
                    if (map[pos + 1] == '#') { dir = 1; code += "R,"; }
                    if (dir == 0) break;
                }
                else if (dir == 1)
                {
                    if (pos - width > 0 && map[pos - width] == '#') { dir = 0; code += "L,"; }
                    if (map[pos + width] == '#') { dir = 2; code += "R,"; }
                    if (dir == 1) break;
                }
                else if (dir == 2)
                {
                    if (map[pos - 1] == '#') { dir = 3; code += "R,"; }
                    if (map[pos + 1] == '#') { dir = 1; code += "L,"; }
                    if (dir == 2) break;
                }
                else if (dir == 3)
                {
                    if (map[pos - width] == '#') { dir = 0; code += "R,"; }
                    if (pos + width < map.Count && map[pos + width] == '#') { dir = 2; code += "L,"; }
                    if (dir == 3) break;
                }
            }

            // Try to find patterns in path
            var fullCode = code;
            code = code.TrimEnd(',');
            var patterns = new List<string>();
            while (code.Length != 0)
            {
                var lastGoodPattern = string.Empty;
                for (var i = 1; i < code.Length; i++)
                {
                    var subs = code.Substring(0, i);

                    if (Regex.Matches(code, subs).Count >= 3 && subs.Length < 20)
                    {
                        lastGoodPattern = subs;
                    }
                    else
                    {
                        code = code.Replace(lastGoodPattern, "").Trim(',');
                        patterns.Add(lastGoodPattern.Trim(','));
                        break;
                    }
                }
            }
            fullCode = fullCode.Replace(patterns[0], "A");
            fullCode = fullCode.Replace(patterns[1], "B");
            fullCode = fullCode.Replace(patterns[2], "C");

            var prg = File.ReadAllText("input.txt").Split(',').Select(long.Parse).ToList();
            prg[0] = 2;
            icc = new IntcodeComputer(prg);
            icc.Output = o => { if (o > 255) Console.WriteLine(o); };

            // debug output
            var feedback = 'n';
            //var y = 0;
            //icc.Output = o =>
            //{
            //    if (o > 255)
            //        Console.WriteLine(o);

            //    if (o == 10)
            //    {
            //        Console.Write('\n');
            //        if (y++ + 1 == height)
            //        {
            //            Console.ReadKey();
            //            Console.Clear();
            //            y = 0;
            //        }
            //    }
            //    else
            //    {
            //        Console.Write((char)o);
            //    }
            //};

            icc.Run(new Stack<long>((fullCode.Trim(',') + "\n").Select(chr => (long)chr).Reverse()));
            icc.Run(new Stack<long>((patterns[0] + "\n").Select(chr => (long)chr).Reverse()));
            icc.Run(new Stack<long>((patterns[1] + "\n").Select(chr => (long)chr).Reverse()));
            icc.Run(new Stack<long>((patterns[2] + "\n").Select(chr => (long)chr).Reverse()));
            icc.Run(new Stack<long>(new[] { (long)'\n', (long)feedback }));
        }
    }
}
