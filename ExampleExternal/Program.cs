using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ExampleExternal
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessEx proc = "Among Us";
            var game = proc["GameAssembly"];
            proc.SetValue(game[0x12345], 0x123321);
        }
    }
}
