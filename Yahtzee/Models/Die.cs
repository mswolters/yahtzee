namespace Yahtzee.Models;

public class Die
{
    internal int Sides { get; }

    internal Die(int sides = 6)
    {
        this.Sides = sides;
    }

    internal DieRoll Roll(Random random)
    {
        return new DieRoll(random.Next(this.Sides) + 1);
    }
}

public readonly record struct DieRoll(int Value)
{
    public override string ToString()
    {
        return Value.ToString();
    }
}