using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var now = DateTime.Now.Ticks;
            var files = System.IO.Directory.GetFiles("f:\\", "*", System.IO.SearchOption.AllDirectories);
            var done = DateTime.Now.Ticks;

            var over = done - now;
            Console.WriteLine(over.ToString());

            Console.ReadKey();
        }

        
    }
}
