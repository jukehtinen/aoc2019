using System;
using System.Collections.Generic;
using System.Linq;

namespace day07
{
    class IntcodeComputer
    {
        public bool IsHalted { get; set; }
        private List<int> _code;
        private int _ip;
        public IntcodeComputer(List<int> code)
        {
            _code = code;
        }

        public int Run(Stack<int> inputs)
        {
            var output = 0;
            while (true)
            {
                var instruction = _code[_ip];
                var op = instruction % 10 + instruction / 10 % 10 * 10;
                var a0 = instruction / 100 % 10;
                var a1 = instruction / 1000 % 10;
                var a2 = instruction / 10000 % 10;

                // add
                if (op == 1)
                {
                    _code[_code[_ip + 3]] = (a0 == 0 ? _code[_code[_ip + 1]] : _code[_ip + 1]) + (a1 == 0 ? _code[_code[_ip + 2]] : _code[_ip + 2]);
                    _ip += 4;
                }
                // mul
                if (op == 2)
                {
                    _code[_code[_ip + 3]] = (a0 == 0 ? _code[_code[_ip + 1]] : _code[_ip + 1]) * (a1 == 0 ? _code[_code[_ip + 2]] : _code[_ip + 2]);
                    _ip += 4;
                }
                // input
                if (op == 3)
                {
                    if (inputs.Count == 0) break;
                    _code[_code[_ip + 1]] = inputs.Pop();
                    _ip += 2;
                }
                // output
                if (op == 4)
                {
                    output = (a0 == 0 ? _code[_code[_ip + 1]] : _code[_ip + 1]);
                    _ip += 2;
                }
                // jump-if-true
                if (op == 5)
                {
                    var p0 = (a0 == 0 ? _code[_code[_ip + 1]] : _code[_ip + 1]);
                    var p1 = (a1 == 0 ? _code[_code[_ip + 2]] : _code[_ip + 2]);
                    if (p0 != 0)
                        _ip = p1;
                    else
                        _ip += 3;
                }
                // jump-if-false
                if (op == 6)
                {
                    var p0 = (a0 == 0 ? _code[_code[_ip + 1]] : _code[_ip + 1]);
                    var p1 = (a1 == 0 ? _code[_code[_ip + 2]] : _code[_ip + 2]);
                    if (p0 == 0)
                        _ip = p1;
                    else
                        _ip += 3;
                }
                // less than
                if (op == 7)
                {
                    var p0 = (a0 == 0 ? _code[_code[_ip + 1]] : _code[_ip + 1]);
                    var p1 = (a1 == 0 ? _code[_code[_ip + 2]] : _code[_ip + 2]);
                    _code[_code[_ip + 3]] = p0 < p1 ? 1 : 0;
                    _ip += 4;
                }
                // equals
                if (op == 8)
                {
                    var p0 = (a0 == 0 ? _code[_code[_ip + 1]] : _code[_ip + 1]);
                    var p1 = (a1 == 0 ? _code[_code[_ip + 2]] : _code[_ip + 2]);
                    _code[_code[_ip + 3]] = p0 == p1 ? 1 : 0;
                    _ip += 4;
                }
                // halt
                if (op == 99)
                {
                    IsHalted = true;
                    break;
                }
            }

            return output;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var code = new List<int> { 3, 8, 1001, 8, 10, 8, 105, 1, 0, 0, 21, 42, 59, 76, 85, 106, 187, 268, 349, 430, 99999, 3, 9, 102, 3, 9, 9, 1001, 9, 2, 9, 1002, 9, 3, 9, 1001, 9, 3, 9, 4, 9, 99, 3, 9, 102, 3, 9, 9, 101, 3, 9, 9, 1002, 9, 2, 9, 4, 9, 99, 3, 9, 102, 3, 9, 9, 1001, 9, 4, 9, 1002, 9, 5, 9, 4, 9, 99, 3, 9, 102, 2, 9, 9, 4, 9, 99, 3, 9, 101, 3, 9, 9, 1002, 9, 2, 9, 1001, 9, 4, 9, 1002, 9, 2, 9, 4, 9, 99, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 1001, 9, 2, 9, 4, 9, 3, 9, 1001, 9, 2, 9, 4, 9, 3, 9, 101, 2, 9, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 101, 1, 9, 9, 4, 9, 3, 9, 101, 1, 9, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 101, 2, 9, 9, 4, 9, 99, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 101, 1, 9, 9, 4, 9, 3, 9, 1001, 9, 2, 9, 4, 9, 3, 9, 1001, 9, 2, 9, 4, 9, 3, 9, 101, 1, 9, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 101, 1, 9, 9, 4, 9, 3, 9, 101, 2, 9, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 101, 2, 9, 9, 4, 9, 99, 3, 9, 1001, 9, 2, 9, 4, 9, 3, 9, 101, 1, 9, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 101, 2, 9, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 101, 2, 9, 9, 4, 9, 3, 9, 101, 2, 9, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 1001, 9, 1, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 99, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 1001, 9, 2, 9, 4, 9, 3, 9, 101, 1, 9, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 101, 1, 9, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 99, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 1001, 9, 1, 9, 4, 9, 3, 9, 101, 1, 9, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 3, 9, 1001, 9, 2, 9, 4, 9, 3, 9, 1001, 9, 2, 9, 4, 9, 3, 9, 102, 2, 9, 9, 4, 9, 3, 9, 1002, 9, 2, 9, 4, 9, 99 };

            // Part 1
            var phases = new[] { 0, 1, 2, 3, 4 };
            var highest = 0;
            do
            {
                var parameter = 0;
                for (var i = 0; i < 5; i++)
                {
                    parameter = new IntcodeComputer(code.ToList()).Run(new Stack<int>(new[] { parameter, phases[i] }));
                }
                if (highest < parameter) highest = parameter;

            } while (Utils.NextPermutation<int>(phases));
            Console.WriteLine(highest);

            // Part 2
            phases = new[] { 5, 6, 7, 8, 9 };
            highest = 0;
            do
            {
                var amps = new List<IntcodeComputer>();
                for (var i = 0; i < 5; i++)
                    amps.Add(new IntcodeComputer(code.ToList()));

                var firstRun = true;
                var parameter = 0;
                while (!amps.Last().IsHalted)
                {
                    for (var i = 0; i < 5; i++)
                    {
                        var inputs = firstRun ? new Stack<int>(new[] { parameter, phases[i] }) : new Stack<int>(new[] { parameter });
                        parameter = amps[i].Run(inputs);
                    }
                    firstRun = false;
                }
                if (highest < parameter) highest = parameter;

            } while (Utils.NextPermutation<int>(phases));
            Console.WriteLine(highest);
        }
    }
}
