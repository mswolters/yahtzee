// See https://aka.ms/new-console-template for more information
using Yahtzee;
using Yahtzee.Models;

Console.WriteLine("Hello, World!");

var state = new GameState();
var player = new ConsolePlayer("Test");
var random = new Random();

while (!state.HasEnded)
{
    await state.DoTurn(player, random);

    Console.WriteLine(state._scoreboard);
}

Console.ReadLine();
