// See https://aka.ms/new-console-template for more information
using Yahtzee;
using Yahtzee.Models;
using Yahtzee.Players.ConsolePlayer;

Console.WriteLine("Hello, World!");

var state = new GameState();
var player = new ConsolePlayer("Test");
var random = new Random();

await state.RunGame(player, random);

Console.ReadLine();
