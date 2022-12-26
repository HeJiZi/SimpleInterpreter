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
        Define(new BuiltinTypeSymbol("INTEGER"));
        Define(new BuiltinTypeSymbol("REAL"));
    }

    public override string ToString()
    {
        return $"Symbols: {string.Join(',', symbols.Select(pair => pair.Value.ToString()))}";

    }

    public void Define(Symbol symbol)
    {
        Console.WriteLine($"Define: {symbol}");
        symbols[symbol.Name] = symbol;
    }

    public Symbol LookUp(string name)
    {
        Console.WriteLine($"LookUp: {name}");
        symbols.TryGetValue(name, out Symbol result);
        return result!;
    }
}