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
                //var result = search.Where(x => !x.IsInstalled && x.Type == UpdateType.utSoftware);
                var temp = search.Search("Type = 'Software'");
            }
            catch (Exception ex)
            {

                throw;
            }
            Console.WriteLine("Hello World!");
        }
    }
}
