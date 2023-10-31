namespace Yahtzee.Models.Rules;

public interface IRule
{
    public RuleId Id { get; }
    public string Name { get; }
    public string Description { get; }

    public bool IsPlayerWritable => this is not IDependOnRules;
    
    public Score Score(IList<DieRoll> rolls, Scoreboard board);
}

public abstract class Rule : IRule
{
    public RuleId Id { get; }
    private readonly string _nameIndex;
    private readonly string _descriptionIndex;

    public string Name => Strings.ResourceManager.GetString(_nameIndex, Strings.Culture)!;

    public string Description => Strings.ResourceManager.GetString(_descriptionIndex, Strings.Culture)!;
    public abstract Score Score(IList<DieRoll> rolls, Scoreboard board);

    public override bool Equals(object? obj)
    {
        return obj is Rule rule &&
               _nameIndex == rule._nameIndex &&
               _descriptionIndex == rule._descriptionIndex;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_nameIndex, _descriptionIndex);
    }

    protected Rule(RuleId id, string nameIndex, string descriptionIndex)
    {
        _nameIndex = nameIndex;
        _descriptionIndex = descriptionIndex;
        Id = id;
    }
}

public interface IDependOnRules : IRule
{
    public int[] DependsOnIndices { get; }
}

public record struct RuleId(string Value);