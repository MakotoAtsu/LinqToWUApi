using System;
using System.Linq.Expressions;
using WUApiLib;
using Xunit;

namespace LinqToWuapi.Test
{
    public class ToSearchStringTest
    {
        [Fact]
        public void Support_BinaryExpression_With_Bool_Constant()
        {
            Expression<Predicate<IUpdate5>> searchTrueExp = (x) => x.IsInstalled == true;
            Expression<Predicate<IUpdate5>> searchFalseExp = (x) => x.IsHidden == false;

            var searchTrue = WuApiExt.ToSearchString(searchTrueExp.Body);
            var searchFalse = WuApiExt.ToSearchString(searchFalseExp.Body);

            Assert.Equal("IsInstalled = 1", searchTrue);
            Assert.Equal("IsHidden = 0", searchFalse);
        }

        [Fact]
        public void Support_BinaryExpression_With_String_Constant()
        {

            Expression<Predicate<IUpdate5>> searchTrueExp = (x) => x.Identity.UpdateID == "111";
            Expression<Predicate<IUpdate5>> searchFalseExp = (x) => x.Identity.UpdateID != "567";

            var searchTrue = WuApiExt.ToSearchString(searchTrueExp.Body);
            var searchFalse = WuApiExt.ToSearchString(searchFalseExp.Body);

            Assert.Equal("UpdateID = '111'", searchTrue);
            Assert.Equal("UpdateID != '567'", searchFalse);
        }

        [Fact]
        public void Support_BinaryExpression_With_Int_Constant()
        {
            // Int constant Only support on RevisionNumber with Type.Equal
            Expression<Predicate<IUpdate5>> searchExp = (x) => x.Identity.RevisionNumber == 15;

            var searchString = WuApiExt.ToSearchString(searchExp.Body);

            Assert.Equal("RevisionNumber = 15", searchString);
        }


        [Fact]
        public void Support_BinaryExpression_With_Two_ChildExpression()
        {
            Expression<Predicate<IUpdate5>> searchExp = (x) => x.IsInstalled == false && x.IsHidden == true;

            Expression<Predicate<IUpdate5>> searchExp2 = (x) => x.IsInstalled == true ||
                                                               x.Identity.RevisionNumber == 10 ||
                                                               x.Identity.UpdateID == "12345";

            var searchString = WuApiExt.ToSearchString(searchExp.Body);
            var searchString2 = WuApiExt.ToSearchString(searchExp2.Body);

            Assert.Equal("(IsInstalled = 0) AND (IsHidden = 1)", searchString);
            Assert.Equal("((IsInstalled = 1) OR (RevisionNumber = 10)) OR (UpdateID = '12345')", searchString2);
        }


        [Fact]
        public void Support_UnaryExpression()
        {
            Expression<Predicate<IUpdate5>> searchExp = (x) => !x.IsInstalled;
            Expression<Predicate<IUpdate5>> search2Exp = (x) => !x.BrowseOnly;
            Expression<Predicate<IUpdate5>> search3Exp = (x) => x.IsHidden;
            Expression<Predicate<IUpdate5>> search4Exp = (x) => x.RebootRequired;


            var searchString = WuApiExt.ToSearchString(searchExp.Body);
            var searchString2 = WuApiExt.ToSearchString(search2Exp.Body);
            var searchString3 = WuApiExt.ToSearchString(search3Exp.Body);
            var searchString4 = WuApiExt.ToSearchString(search4Exp.Body);

            Assert.Equal("IsInstalled = 0", searchString);
            Assert.Equal("BrowseOnly = 0", searchString2);
            Assert.Equal("IsHidden = 1", searchString3);
            Assert.Equal("RebootRequired = 1", searchString4);

        }

    }
}
