namespace Yahtzee.Models;

public interface IWrapper<T>
{
    public T Value { get; init; }
}