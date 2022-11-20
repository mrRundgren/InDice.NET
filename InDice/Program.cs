// See https://aka.ms/new-console-template for more information
using InDice;
using InDice.NET;

Console.WriteLine("Hello, World!");

var tmp = new SomeModel { Firstname = "Anders", Lastname = "Rundgren", Street = "Vångavägen 10", Zip = "24132", City = "Eslöv" };

var gen = new IndexGenerator("");
var indexes = gen.Generate(tmp);

foreach(var index in indexes)
{
    Console.WriteLine($"{index.Key} ({index.Value})");
}

Console.ReadLine();

