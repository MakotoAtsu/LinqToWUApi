using System;
using System.Diagnostics;
using System.Linq.Expressions;
using LinqToWuapi;
using WUApiLib;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var search = new UpdateSession().CreateUpdateSearcher();
            search.Online = false;


            try
            {
                var result = search.Where(x => x.IsInstalled);

                foreach (IUpdate item in result.Updates)
                {
                    Console.WriteLine($"{item.Categories} :");
                    foreach (ICategory cat in item.Categories)
                    {
                        Console.WriteLine(cat.CategoryID);
                    }
                }

                var temp = search.Search("CategoryIDs contains 'e0789628-ce08-4437-be74-2495b842f43b'");
            }
            catch (Exception ex)
            {

                throw;
            }
            Console.WriteLine("Hello World!");
        }
    }
}
