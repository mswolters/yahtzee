namespace Yahtzee.Models.Rules;

public abstract class Rule
{
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

    public Rule(String nameIndex, String descriptionIndex)
    {
        _nameIndex = nameIndex;
        _descriptionIndex = descriptionIndex;
    }
}