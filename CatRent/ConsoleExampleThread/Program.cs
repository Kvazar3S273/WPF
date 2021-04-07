using CatRenta.Application.Interfaces;
using CatRenta.Infrastructure.Services;
using System;
using System.Threading;

namespace ConsoleExampleThread
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread catThread = new Thread(InsertCat);
            catThread.Start();
        }
        static void InsertCat()
        {
            int count;
            ICatService catService = new CatService();
            Console.WriteLine("На початку роботи котів: " + catService.Count());
            Console.WriteLine("Скільки котів додати в БД?");
            count = int.Parse(Console.ReadLine());
            catService.InsertCats(count);
            Console.WriteLine("В кінці роботи котів: " + catService.Count());
        }
    }
}
