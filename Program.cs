using SimpleInterpreter.Core;
using SimpleInterpreter.Tool;

namespace SimpleInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            string curLine;
            string program = @"
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

            var lexer = new Lexer(program);
            var parser = new Parser(lexer);
            var interpreter = new Interpreter(parser);
            interpreter.Interprete();
            interpreter.PrintVars();
            
            PrintTokens(new Lexer(program));
            AstVisualUtil.PrintTree(new Parser(new Lexer(program)).Parse());

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
    }



    
}