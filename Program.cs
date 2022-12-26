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
            string program1 = @"
            PROGRAM Part10AST;
            VAR
               a, b : INTEGER;
               y    : REAL;
               z    : REAL;

            BEGIN {Part10AST}
               a := 2;
               b := 10 * a + 10 * a DIV 4;
               y := 20 / 7 + 3.14;
               z := y * 5 - a;
            END.  {Part10AST}";

            string program2 = @"
            PROGRAM Part11;
            VAR
               number : INTEGER;
               a, b   : INTEGER;
               y      : REAL;

            BEGIN {Part11}
               number := 2;
               a := number ;
               b := 10 * a + 10 * number DIV 4;
               y := 20 / 7 + 3.14
            END.  {Part11}";

            string nameError = @"
            PROGRAM NameError1;
            VAR
               b : INTEGER;

            BEGIN
               b := 1;
               a := 2 + b;
            END.";
            return program2;
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