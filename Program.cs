using System.Reflection;
using SimpleInterpreter.Core;
using SimpleInterpreter.Tool;

namespace SimpleInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            // TestSymbol();
            string curLine;
            string program = GetProgram();

            var lexer = new Lexer(program);
            var parser = new Parser(lexer);
            var symtabBuilder = new SymbolTableBuilder();
            var root = parser.Parse();
            symtabBuilder.Visit(root);
            Console.WriteLine(symtabBuilder);
            var interpreter = new Interpreter(parser);
            // interpreter.Interprete();
            interpreter.Visit(root);
            interpreter.PrintVars();

            // PrintTokens(new Lexer(program));
            // AstVisualUtil.PrintTree(new Parser(new Lexer(program)).Parse());

        }

        static string GetProgram()
        {
            Console.WriteLine("Program:>");
            string programName = "part12";
            string path = $"../../../Scripts/{programName}.pas";
            string program = File.ReadAllText(path);
            Console.WriteLine(program);
            return program;
        }

        static void PrintTokens(Lexer lexer)
        {
            Console.WriteLine("\nStart:Tokens>");
            var token = lexer.GetNextToken();
            while (token.Type != TokenType.Eof)
            {
                Console.WriteLine(token);
                token = lexer.GetNextToken();
            }
        }

        static void TestSymbol()
        {
            var intType = new BuiltinTypeSymbol("INTEGER");
            var realType = new BuiltinTypeSymbol("REAL");

            var xSymbol = new VarSymbol("x", intType);
            var ySymbol = new VarSymbol("y", realType);
            Console.WriteLine(intType);
            Console.WriteLine(realType);
            Console.WriteLine(xSymbol);
            Console.WriteLine(ySymbol);
        }
    }



    
}