﻿namespace Yahtzee.Models.Rules;

internal class YahtzeeRule : Rule
{
    public YahtzeeRule(RuleId id) : base(id, "RuleNameYahtzee", "RuleDescriptionYahtzee")
    {
    }

    public override Score Score(IList<DieRoll> rolls, Scoreboard board)
    {
        if (rolls.GroupBy(roll => roll.Value).Count() == 1)
        {
            // TODO: Special case multiple Yahtzees somehow
            return new Score(50);
        }
        return new Score(0);
    }
    public override bool Equals(object? obj)
    {
        return obj is YahtzeeRule &&
               base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode());
    }

}