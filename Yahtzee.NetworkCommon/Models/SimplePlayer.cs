using System.ComponentModel;
using Yahtzee.Players;

namespace Yahtzee.NetworkCommon.Models;

// A simple property model for sending player information over the network
public class SimplePlayer : IPlayer
{
    public Guid Id { get; init; }
    public string Name { get; set; } = "";
    
    public event PropertyChangedEventHandler? PropertyChanged;

    public static SimplePlayer Of(IPlayer player)
    {
        return new SimplePlayer
        {
            Id = player.Id,
            Name = player.Name
        };
    }
}