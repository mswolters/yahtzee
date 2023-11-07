// See https://aka.ms/new-console-template for more information
using Yahtzee;
using Yahtzee.Models;
using Yahtzee.Players.ConsolePlayer;

Console.WriteLine("Hello, World!");

var state = new GameManager();
var player1 = new ConsolePlayer("Test");
var player2 = new ConsolePlayer("Test2");
var random = new Random();

await state.RunGame(random, player1, player2);

Console.ReadLine();
