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
            var search = new WUApiLib.UpdateSession().CreateUpdateSearcher();
            search.Online = false;

            //var result = search.Search("IsInstalled = 1 and BrowseOnly = 0").Updates;

            //foreach (IUpdate item in result)
            //{
            //    Debug.WriteLine($"{item.Title} : {item.IsMandatory}");
            //}
            Expression<Predicate<IUpdate4>> aa = x => x.BrowseOnly == true &&
                                                     (x.IsInstalled == true || x.IsHidden == false);
            var res = aa.ReduceExtensions();

            var sss = LinqToWuapi.WuApiExt.ToSearchString(aa.Body);



            try
            {
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
