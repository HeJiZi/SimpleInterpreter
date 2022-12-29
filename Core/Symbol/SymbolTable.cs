using System.Collections.Concurrent;
using System.Collections.Specialized;

namespace SimpleInterpreter.Core;

public class SymbolTable
{
    private Dictionary<string, Symbol> symbols; 
    
    public SymbolTable()
    {
        symbols = new Dictionary<string, Symbol>();
        InitBuiltIns();
    }
    
    private void InitBuiltIns()
    {
        Insert(new BuiltinTypeSymbol("INTEGER"));
        Insert(new BuiltinTypeSymbol("REAL"));
    }

    public override string ToString()
    {
        var symtabHeader = "Symbol table contents";
        var lines = new List<string>() { "\n", symtabHeader, new('_', symtabHeader.Length) };
        lines.AddRange(symbols.Select(pair => $"{pair.Key,+7}: {pair.Value}"));
        lines.Add("\n");
        return string.Join('\n', lines);

    }

    public void Insert(Symbol symbol)
    {
        Console.WriteLine($"Insert: {symbol}");
        symbols[symbol.Name] = symbol;
    }

    public Symbol LookUp(string name)
    {
        Console.WriteLine($"LookUp: {name}");
        symbols.TryGetValue(name, out Symbol result);
        return result;
    }
}