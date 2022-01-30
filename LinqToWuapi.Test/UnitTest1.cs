using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using WUApiLib;
using Xunit;

namespace LinqToWuapi.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Support_BinaryExpression_With_Bool_Constant()
        {
            Expression<Predicate<IUpdate>> searchTrueExp = (x) => x.IsInstalled == true;
            Expression<Predicate<IUpdate>> searchFalseExp = (x) => x.IsHidden == false;

            var searchTrue = WuApiExt.ToSearchString(searchTrueExp.Body);
            var searchFalse = WuApiExt.ToSearchString(searchFalseExp.Body);

            Assert.Equal("IsInstalled = 1", searchTrue);
            Assert.Equal("IsHidden = 0", searchFalse);
        }

        [Fact]
        public void Support_BinaryExpression_With_String_Constant()
        {

            Expression<Predicate<IUpdate>> searchTrueExp = (x) => x.Identity.UpdateID == "111";
            Expression<Predicate<IUpdate>> searchFalseExp = (x) => x.Identity.UpdateID != "567";

            var searchTrue = WuApiExt.ToSearchString(searchTrueExp.Body);
            var searchFalse = WuApiExt.ToSearchString(searchFalseExp.Body);

            Assert.Equal("UpdateID = '111'", searchTrue);
            Assert.Equal("UpdateID != '567'", searchFalse);
        }

        [Fact]
        public void Support_BinaryExpression_With_Int_Constant()
        {
            // Int constant Only support on RevisionNumber with Type.Equal
            Expression<Predicate<IUpdate>> searchExp = (x) => x.Identity.RevisionNumber == 15;

            var searchString = WuApiExt.ToSearchString(searchExp.Body);

            Assert.Equal("RevisionNumber = 15", searchString);
        }


        [Fact]
        public void Support_BinaryExpression_With_Two_ChildExpression()
        {
            Expression<Predicate<IUpdate>> searchExp = (x) => x.IsInstalled == false && x.IsHidden == true;

            Expression<Predicate<IUpdate>> searchExp2 = (x) => x.IsInstalled == true ||
                                                               x.Identity.RevisionNumber == 10 ||
                                                               x.Identity.UpdateID == "12345";

            var searchString = WuApiExt.ToSearchString(searchExp.Body);
            var searchString2 = WuApiExt.ToSearchString(searchExp2.Body);

            Assert.Equal("IsInstalled = 0 AND IsHidden = 1", searchString);
            Assert.Equal("IsInstalled = 1 OR RevisionNumber = 10 OR UpdateID = '12345'", searchString2);
        }


        [Fact]
        public void Support_BinaryExpression_With_ChildExpression_And_Nested()
        {
            Expression<Predicate<IUpdate>> sear = (x) => x.IsInstalled == true && x.IsHidden == true;
            Expression<Predicate<IUpdate>> searchExp = (x) => (x.IsInstalled == true && x.IsHidden == true) &&
                                                              x.AutoSelectOnWebSites == true;



            var searchString = WuApiExt.ToSearchString(sear.Body);


        }
    }
}
