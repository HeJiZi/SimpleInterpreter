using System.Reflection;
using SimpleInterpreter.Core;
using SimpleInterpreter.Exceptions;
using SimpleInterpreter.Tool;

namespace SimpleInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {

            // TestSymbol();
            // TestStack();
            LogUtil.OpenLog = false;
            
            string curLine;
            string program = GetProgram();

            var lexer = new Lexer(program);
            var parser = new Parser(lexer);
            var semanticAnalyzer = new SemanticAnalyzer();
            var root = parser.Parse();
            semanticAnalyzer.Visit(root);
            var interpreter = new Interpreter(parser);
            Interpreter.ShouldLogStack = true;
            // interpreter.Interprete();
            interpreter.Visit(root);
            // interpreter.PrintVars();

            // PrintTokens(new Lexer(program));
            // AstVisualUtil.PrintTree(root);

        }

        static string GetProgram(bool print = false)
        {
            
            string programName = "part17";
            string path = $"../../../Scripts/{programName}.pas";
            string program = File.ReadAllText(path);
            if (print)
            {
                Console.WriteLine("Program:>");
                Console.WriteLine(program);
            }
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

        static void TestStack()
        {
            var stack = new CallStack();
            var ar = new ActivationRecord("Main", ARType.RPOGRAM, 1);
            Console.WriteLine(ar);
            ar["y"] = 7;
            Console.WriteLine(ar);
            stack.Push(ar);
            Console.WriteLine(stack);
        }
    }



    
}