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
                var result = search.Where(x => !x.IsInstalled && x.BrowseOnly == false);
                var temp = search.Search("(BrowseOnly = 1 OR IsHidden = 1) AND IsInstalled = 1");
            }
            catch (Exception ex)
            {

                throw;
            }
            Console.WriteLine("Hello World!");
        }
    }
}
