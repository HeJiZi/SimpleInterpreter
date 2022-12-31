namespace SimpleInterpreter.Core;

public enum ARType 
{
    RPOGRAM,
    PROCEDURE,
}

public class ActivationRecord
{
    public string Name { get; }

    public ARType Type { get; }

    public int NestingLevel { get; }

    private Dictionary<string, dynamic> members;

    public ActivationRecord(string name, ARType type, int nestingLevel)
    {
        Name = name;
        Type = type;
        NestingLevel = nestingLevel;
        members = new Dictionary<string, dynamic>(1000);
    }

    public dynamic this[string index]
    {
        get
        {
            dynamic result = null;
            members.TryGetValue(index, out result);
            return result;
        }
        set
        {
            members[index] = value;
        }
    }

    public override string ToString()
    {
        var lines = new List<string>()
        {
            $"{NestingLevel}: {Type} {Name}",
        };
        lines.AddRange(members.Select(pair=> $"   {pair.Key, -20}: {pair.Value}"));

        return string.Join('\n', lines);
    }
}