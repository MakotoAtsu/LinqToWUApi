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
        public void Support_BinaryExpression_With_Enum_Constant()
        {

            Expression<Predicate<IUpdate5>> searchExp = (x) => x.Type == UpdateType.utSoftware;
            Expression<Predicate<IUpdate5>> searchExp2 = (x) => x.Type == UpdateType.utDriver;

            Expression<Predicate<IUpdate5>> searchExp3 = (x) => x.DeploymentAction == DeploymentAction.daNone;
            Expression<Predicate<IUpdate5>> searchExp4 = (x) => x.DeploymentAction == DeploymentAction.daDetection;
            Expression<Predicate<IUpdate5>> searchExp5 = (x) => x.DeploymentAction == DeploymentAction.daUninstallation;
            Expression<Predicate<IUpdate5>> searchExp6 = (x) => x.DeploymentAction == DeploymentAction.daOptionalInstallation;
            Expression<Predicate<IUpdate5>> searchExp7 = (x) => x.DeploymentAction == DeploymentAction.daInstallation;

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
