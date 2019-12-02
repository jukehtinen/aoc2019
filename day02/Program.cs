using System;
using System.Collections.Generic;
using System.Linq;

namespace day02
{
    class Program
    {
        static void RunCode(List<int> code)
        {
            var ip = 0;
            while (true)
            {
                var op = code[ip];
                if (op == 1)
                {
                    code[code[ip + 3]] = code[code[ip + 1]] + code[code[ip + 2]];
                }

                if (op == 2)
                {
                    code[code[ip + 3]] = code[code[ip + 1]] * code[code[ip + 2]];
                }

                if (op == 99)
                {
                    break;
                }

                ip += 4;
            }
        }

        static void Main(string[] args)
        {
            var code = new List<int> { 1, 0, 0, 3, 1, 1, 2, 3, 1, 3, 4, 3, 1, 5, 0, 3, 2, 13, 1, 19, 1, 6, 19, 23, 2, 23, 6, 27, 1, 5, 27, 31, 1, 10, 31, 35, 2, 6, 35, 39, 1, 39, 13, 43, 1, 43, 9, 47, 2, 47, 10, 51, 1, 5, 51, 55, 1, 55, 10, 59, 2, 59, 6, 63, 2, 6, 63, 67, 1, 5, 67, 71, 2, 9, 71, 75, 1, 75, 6, 79, 1, 6, 79, 83, 2, 83, 9, 87, 2, 87, 13, 91, 1, 10, 91, 95, 1, 95, 13, 99, 2, 13, 99, 103, 1, 103, 10, 107, 2, 107, 10, 111, 1, 111, 9, 115, 1, 115, 2, 119, 1, 9, 119, 0, 99, 2, 0, 14, 0 };
            
            // Part 1
            var copy = code.ToList();
            copy[1] = 12;
            copy[2] = 2;
            RunCode(copy);
            Console.WriteLine(copy[0]);

            // Part 2                
            for (var v = 0; v < 100; v++)
            {
                for (var n = 0; n < 100; n++)
                {
                    copy = code.ToList();
                    copy[1] = n;
                    copy[2] = v;
                    RunCode(copy);

                    if (copy[0] == 19690720)
                    {
                        Console.WriteLine(100 * n + v);
                        return;
                    }
                }
            }
        }
    }
}
