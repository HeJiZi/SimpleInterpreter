using System.Collections.Concurrent;
using System.Collections.Specialized;

namespace SimpleInterpreter.Core;

public class ScopedSymbolTable
{
    private Dictionary<string, Symbol> symbols;
    private string scopeName;
    private int scopeLevel;
    private ScopedSymbolTable enclosingScope;

    public ScopedSymbolTable EnclosingScope => enclosingScope;

    public int ScopeLeve => scopeLevel;
    
    public ScopedSymbolTable(string scopeName, int scopeLevel, ScopedSymbolTable enclosingScope = null)
    {
        symbols = new Dictionary<string, Symbol>();
        this.scopeName = scopeName;
        this.scopeLevel = scopeLevel;
        this.enclosingScope = enclosingScope;
        InitBuiltIns();
    }
    
    private void InitBuiltIns()
    {
        Insert(new BuiltinTypeSymbol("INTEGER"));
        Insert(new BuiltinTypeSymbol("REAL"));
    }

    public override string ToString()
    {
        var h1 = "SCOPE (SCOPED SYMBOL TABLE)";
        var lines = new List<string>() { "\n", h1, new('_', h1.Length) };
        string s = "Scope name";
        lines.Add($"{s, -15}: {scopeName}");
        s = "Scope level";
        lines.Add($"{s, -15}: {scopeLevel}");
        s = "Enclosing scope";
        lines.Add($"{s, -15}: {enclosingScope?.scopeName}");

        var h2 = "Scope (Scoped symbol table) contents";
        lines.AddRange(new[]{ h2, new('_', h2.Length) });
        lines.AddRange(symbols.Select(pair => $"{pair.Key,+7}: {pair.Value}"));
        lines.Add("\n");
        return string.Join('\n', lines);

    }

    public void Insert(Symbol symbol)
    {
        Console.WriteLine($"Insert: {symbol}");
        symbols[symbol.Name] = symbol;
    }

    public Symbol LookUp(string name, bool currentScopeOnly = false)
    {
        Console.WriteLine($"Lookup: {name}. (Scope name: {scopeName})");
        symbols.TryGetValue(name, out Symbol result);
        if (result != null || currentScopeOnly)
            return result;
        
        if (enclosingScope != null)
            return enclosingScope.LookUp(name);
        
        return result;
    }
}