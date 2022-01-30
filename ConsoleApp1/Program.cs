using System;
using LinqToWuapi;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var search = new WUApiLib.UpdateSession().CreateUpdateSearcher();
            search.Online = false;
            //var result = search.Where(x => x.IsInstalled == true);
            try
            {
                var temp = search.Search("BrowseOnly = 1");
            }
            catch (Exception ex)
            {

                throw;
            }
            Console.WriteLine("Hello World!");
        }
    }
}
