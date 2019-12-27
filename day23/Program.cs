using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace day23
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

    class NIC
    {
        private IntcodeComputer _icc;
        private List<long> _data = new List<long>();
        private int _id;
        public Queue<(long x, long y)> Queue { get; set; } = new Queue<(long x, long y)>();
        public Action<(int from, int target, long x, long y)> Message { get; set; }
        public int IdleCount { get; set; }
        public NIC(int id)
        {
            _id = id;
            _icc = new IntcodeComputer(File.ReadAllText("input.txt").Split(',').Select(long.Parse).ToList());
            _icc.Output = o =>
            {
                _data.Add(o);
                if (_data.Count == 3)
                {
                    Message.Invoke((_id, (int)_data[0], _data[1], _data[2]));
                    _data.Clear();
                }
            };
            _icc.Run(new Stack<long>(new long[] { _id }));
        }

        public void Run()
        {
            if (Queue.Count != 0)
            {
                IdleCount = 0;
                while (Queue.TryDequeue(out (long x, long y) res))
                {
                    _icc.Run(new Stack<long>(new long[] { res.y, res.x }));
                }
            }
            else
            {
                IdleCount++;
            }

            _icc.Run(new Stack<long>(new long[] { -1 }));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var part1FirstMessage = false;

            (long x, long y) natMessage = (0, 0);

            var nics = new List<NIC>();
            for (var i = 0; i < 50; i++)
            {
                var nic = new NIC(i);
                nic.Message = ((int from, int target, long x, long y) msg) =>
                {
                    if (msg.target == 255)
                    {
                        if (!part1FirstMessage)
                        {
                            Console.WriteLine(msg.y);
                            part1FirstMessage = true;
                        }

                        natMessage = (msg.x, msg.y);
                    }
                    else
                        nics[msg.target].Queue.Enqueue((msg.x, msg.y));
                };
                nics.Add(nic);
            }

            long lastNatY = 0;
            while (true)
            {
                nics.ForEach(n => n.Run());

                if (nics.All(n => n.IdleCount > 10))
                {
                    if (lastNatY == natMessage.y)
                        Console.WriteLine(natMessage.y);
                    lastNatY = natMessage.y;
                    nics[0].Queue.Enqueue(natMessage);
                }
            }
        }
    }
}
