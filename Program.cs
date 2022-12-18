using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleInterpreter.Core;

namespace SimpleInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            string curLine;
            while ((curLine = Console.ReadLine()) != null)
            {
                var lexer = new Lexer(curLine);
                var parser = new Parser(lexer);
                Console.WriteLine(new Interpreter(parser).Interprete());
            }
        }
    }



    
}