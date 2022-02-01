using System;
using System.Linq.Expressions;
using Xunit;

namespace LinqToWuapi.Test
{
    public class ToSearchStringTest
    {
        [Fact]
        public void Support_BinaryExpression_With_Bool_Constant()
        {
            Expression<Predicate<ISearchObject>> searchTrueExp = (x) => x.IsInstalled == true;
            Expression<Predicate<ISearchObject>> searchFalseExp = (x) => x.IsHidden == false;

            var searchTrue = WuApiExt.ToSearchString(searchTrueExp.Body);
            var searchFalse = WuApiExt.ToSearchString(searchFalseExp.Body);

            Assert.Equal("IsInstalled = 1", searchTrue);
            Assert.Equal("IsHidden = 0", searchFalse);
        }

        [Fact]
        public void Support_BinaryExpression_With_String_Constant()
        {

            Expression<Predicate<ISearchObject>> searchTrueExp = (x) => x.UpdateID == "111";
            Expression<Predicate<ISearchObject>> searchFalseExp = (x) => x.UpdateID != "567";

            var searchTrue = WuApiExt.ToSearchString(searchTrueExp.Body);
            var searchFalse = WuApiExt.ToSearchString(searchFalseExp.Body);

            Assert.Equal("UpdateID = '111'", searchTrue);
            Assert.Equal("UpdateID != '567'", searchFalse);
        }

        [Fact]
        public void Support_BinaryExpression_With_Int_Constant()
        {
            // Int constant Only support on RevisionNumber with Type.Equal
            Expression<Predicate<ISearchObject>> searchExp = (x) => x.RevisionNumber == 15;

            var searchString = WuApiExt.ToSearchString(searchExp.Body);

            Assert.Equal("RevisionNumber = 15", searchString);
        }


        [Fact]
        public void Support_BinaryExpression_With_Two_ChildExpression()
        {
            Expression<Predicate<ISearchObject>> searchExp = (x) => x.IsInstalled == false && x.IsHidden == true;

            Expression<Predicate<ISearchObject>> searchExp2 = (x) => x.IsInstalled == true ||
                                                               x.RevisionNumber == 10 ||
                                                               x.UpdateID == "12345";

            var searchString = WuApiExt.ToSearchString(searchExp.Body, true);
            var searchString2 = WuApiExt.ToSearchString(searchExp2.Body, true);

            Assert.Equal("(IsInstalled = 0) AND (IsHidden = 1)", searchString);
            Assert.Equal("((IsInstalled = 1) OR (RevisionNumber = 10)) OR (UpdateID = '12345')", searchString2);
        }

        [Fact]
        public void Support_BinaryExpression_With_Enum_Constant()
        {

            Expression<Predicate<ISearchObject>> searchExp = (x) => x.Type == sUpdateType.Software;
            Expression<Predicate<ISearchObject>> searchExp2 = (x) => x.Type == sUpdateType.Driver;

            Expression<Predicate<ISearchObject>> searchExp3 = (x) => x.DeploymentAction == sDeploymentAction.None;
            Expression<Predicate<ISearchObject>> searchExp4 = (x) => x.DeploymentAction == sDeploymentAction.Detection;
            Expression<Predicate<ISearchObject>> searchExp5 = (x) => x.DeploymentAction == sDeploymentAction.Uninstallation;
            Expression<Predicate<ISearchObject>> searchExp6 = (x) => x.DeploymentAction == sDeploymentAction.OptionalInstallation;
            Expression<Predicate<ISearchObject>> searchExp7 = (x) => x.DeploymentAction == sDeploymentAction.Installation;

            var searchString = WuApiExt.ToSearchString(searchExp.Body);
            var searchString2 = WuApiExt.ToSearchString(searchExp2.Body);
            var searchString3 = WuApiExt.ToSearchString(searchExp3.Body);
            var searchString4 = WuApiExt.ToSearchString(searchExp4.Body);
            var searchString5 = WuApiExt.ToSearchString(searchExp5.Body);
            var searchString6 = WuApiExt.ToSearchString(searchExp6.Body);
            var searchString7 = WuApiExt.ToSearchString(searchExp7.Body);


            Assert.Equal("Type = 'Software'", searchString);
            Assert.Equal("Type = 'Driver'", searchString2);

            Assert.Equal("DeploymentAction = 'None'", searchString3);
            Assert.Equal("DeploymentAction = 'Detection'", searchString4);
            Assert.Equal("DeploymentAction = 'Uninstallation'", searchString5);
            Assert.Equal("DeploymentAction = 'OptionalInstallation'", searchString6);
            Assert.Equal("DeploymentAction = 'Installation'", searchString7);

        }

        [Fact]
        public void Support_UnaryExpression()
        {
            Expression<Predicate<ISearchObject>> searchExp = (x) => !x.IsInstalled;
            Expression<Predicate<ISearchObject>> search2Exp = (x) => !x.BrowseOnly;
            Expression<Predicate<ISearchObject>> search3Exp = (x) => x.IsHidden;
            Expression<Predicate<ISearchObject>> search4Exp = (x) => x.RebootRequired;


            var searchString = WuApiExt.ToSearchString(searchExp.Body);
            var searchString2 = WuApiExt.ToSearchString(search2Exp.Body);
            var searchString3 = WuApiExt.ToSearchString(search3Exp.Body);
            var searchString4 = WuApiExt.ToSearchString(search4Exp.Body);

            Assert.Equal("IsInstalled = 0", searchString);
            Assert.Equal("BrowseOnly = 0", searchString2);
            Assert.Equal("IsHidden = 1", searchString3);
            Assert.Equal("RebootRequired = 1", searchString4);

        }


        [Fact]
        public void Support_Nested_Expression()
        {
            Expression<Predicate<ISearchObject>> searchExp = (x) => (x.BrowseOnly == true && x.IsInstalled == true) ||
                                                               (x.BrowseOnly == false && x.IsInstalled == false);

            Expression<Predicate<ISearchObject>> searchExp2 = (x) => x.BrowseOnly || x.IsHidden || x.IsInstalled;

            Expression<Predicate<ISearchObject>> searchExp3 = (x) => x.IsHidden && x.CategoryIDs.Contains("12345");

            var searchString = WuApiExt.ToSearchString(searchExp.Body, true);
            var searchString2 = WuApiExt.ToSearchString(searchExp2.Body, true);
            var searchString3 = WuApiExt.ToSearchString(searchExp3.Body, true);

            Assert.Equal("((BrowseOnly = 1) AND (IsInstalled = 1)) OR ((BrowseOnly = 0) AND (IsInstalled = 0))",
                         searchString);

            Assert.Equal("((BrowseOnly = 1) OR (IsHidden = 1)) OR (IsInstalled = 1)",
                         searchString2);

            Assert.Equal("(IsHidden = 1) AND (CategoryIDs contains '12345')", searchString3);
        }

        [Fact]
        public void NotSupport_Nested_Expression_With_OrType_without_Top_Level()
        {
            Expression<Predicate<ISearchObject>> searchExp = (x) => (x.IsInstalled == true || x.IsHidden) &&
                                                               x.RebootRequired == true;

            Expression<Predicate<ISearchObject>> searchExp2 = (x) => (x.BrowseOnly && x.IsInstalled) &&
                                                                (x.IsHidden || x.RebootRequired);

            var argEx = Assert.Throws<ArgumentException>(() => WuApiExt.ToSearchString(searchExp.Body, true));
            var argEx2 = Assert.Throws<ArgumentException>(() => WuApiExt.ToSearchString(searchExp2.Body, true));

            var expectedExMsg = "OR can be used only at the top level of the search criteria";
            Assert.Equal(expectedExMsg, argEx.Message);
            Assert.Equal(expectedExMsg, argEx2.Message);
        }

        [Fact]
        public void NotSupport_Invalid_Operator()
        {
            Expression<Predicate<ISearchObject>> searchExp = (x) => x.BrowseOnly != true;
            Expression<Predicate<ISearchObject>> searchExp2 = (x) => x.IsHidden != false;

            Assert.Throws<ArgumentException>(() => WuApiExt.ToSearchString(searchExp.Body, true));
            Assert.Throws<ArgumentException>(() => WuApiExt.ToSearchString(searchExp2.Body, true));
        }


        [Fact]
        public void Support_Search_CategoryIDs_with_Contains()
        {
            Expression<Predicate<ISearchObject>> searchExp = (x) => x.CategoryIDs.Contains("12345");

            var searchString = WuApiExt.ToSearchString(searchExp.Body, true);

            Assert.Equal("CategoryIDs contains '12345'", searchString);
        }


    }
}
