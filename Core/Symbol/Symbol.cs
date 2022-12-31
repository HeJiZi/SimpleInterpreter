namespace SimpleInterpreter.Core;

public class Symbol
{
    public string Name { get; }

    public Symbol Type { get; }

    public int ScopeLevel = 0;

    public Symbol(string name, Symbol type = null)
    {
        Name = name;
        Type = type;
        ScopeLevel = 0;
    }
}

public class BuiltinTypeSymbol:Symbol
{
    public BuiltinTypeSymbol(string name) : base(name)
    {
    }

    public override string ToString()
    {
        return $"<{GetType().Name}(name='{Name}')>";
    }
}

public class VarSymbol : Symbol
{
    public VarSymbol(string name, Symbol type) : base(name, type)
    {
        
    }

    public override string ToString()
    {
        return $"<{GetType().Name}(name='{Name}', type='{Type.Name}')>";
    }
}

public class ProcedureSymbol : Symbol
{
    public List<VarSymbol> Params { get; }
    public Block BlockAst { get; set; }

    public ProcedureSymbol(string name, List<VarSymbol> pParams = null) : base(name)
    {
        Params = pParams == null ? new List<VarSymbol>() : pParams;
    }

    public override string ToString()
    {
        return $"<{GetType().Name}(name={Name}, parameters=[{string.Join(',', Params)}])>";
    }
}