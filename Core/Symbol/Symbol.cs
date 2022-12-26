namespace SimpleInterpreter.Core;

public class Symbol
{
    public string Name { get; }

    public Symbol Type { get; }

    public Symbol(string name, Symbol type = null)
    {
        Name = name;
        Type = type;
    }
}

public class BuiltinTypeSymbol:Symbol
{
    public BuiltinTypeSymbol(string name) : base(name)
    {
    }

    public override string ToString()
    {
        return Name;
    }
}

public class VarSymbol : Symbol
{
    public VarSymbol(string name, Symbol type) : base(name, type)
    {
        
    }

    public override string ToString()
    {
        return $"<{Name}:{Type}>";
    }
}