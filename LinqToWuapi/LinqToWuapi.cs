using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using WUApiLib;

namespace LinqToWuapi
{
    public static class WuApiExt
    {
        private static readonly Dictionary<ExpressionType, string> ExpressionTypeMapping = new Dictionary<ExpressionType, string>
        {
            { ExpressionType.And, "AND" },
            { ExpressionType.AndAlso, "AND" },
            { ExpressionType.Or, "OR" },
            { ExpressionType.OrElse, "OR" },
            { ExpressionType.Equal , "=" },
            { ExpressionType.NotEqual , "!=" },
        };

        private static readonly Dictionary<string, HashSet<ExpressionType>> MemberAllowType = new Dictionary<string, HashSet<ExpressionType>>(StringComparer.OrdinalIgnoreCase)
        {
            {"Type",new HashSet<ExpressionType>(){ ExpressionType.Equal,ExpressionType.NotEqual} },
            {"DeploymentAction",new HashSet<ExpressionType>(){ ExpressionType.Equal} },
            {"IsAssigned",new HashSet<ExpressionType>(){ ExpressionType.Equal} },
            {"BrowseOnly",new HashSet<ExpressionType>(){ ExpressionType.Equal} },
            {"AutoSelectOnWebSites",new HashSet<ExpressionType>(){ ExpressionType.Equal} },
            {"UpdateID",new HashSet<ExpressionType>(){ ExpressionType.Equal,ExpressionType.NotEqual} },
            {"RevisionNumber",new HashSet<ExpressionType>(){ ExpressionType.Equal} },
            {"CategoryIDs",new HashSet<ExpressionType>(){ ExpressionType.Constant} }, //Container
            {"IsInstalled", new HashSet<ExpressionType>(){ExpressionType.Equal,ExpressionType.NotEqual} },
            {"IsHidden",new HashSet<ExpressionType>(){ ExpressionType.Equal} },
            {"IsPresent",new HashSet<ExpressionType>(){ ExpressionType.Equal} },
            {"RebootRequired",new HashSet<ExpressionType>(){ ExpressionType.Equal} },

        };

        public static IUpdate Where(this IUpdateSearcher searcher, Expression<Predicate<IUpdate>> expression)
        {


            throw new NotImplementedException();
        }

        public static string ToSearchString(Expression expression)
        {

            if (expression is BinaryExpression binExp)
            {
                return ExtractBinaryExpression(binExp);
            }
            else
            {
                throw new NotImplementedException("Only support BinaryExpression");
            }

            throw new NotImplementedException();
        }


        private static string ExtractBinaryExpression(BinaryExpression binExp)
        {

            // if Right is Constant , Left must be Member
            if (binExp.Right is ConstantExpression constantExp)
            {
                if (binExp.Left is MemberExpression memberExp)
                {
                    var mamberName = memberExp.Member.Name;

                    if (!MemberAllowType.ContainsKey(mamberName))
                        throw new Exception($"Not support serach attribube {mamberName}");

                    // Check Member support Expression Type
                    if (!MemberAllowType[mamberName].Contains(binExp.NodeType))
                        throw new Exception($"The attribute:{mamberName} " +
                                            $"only support expression:{string.Join(",", MemberAllowType[mamberName])}");

                    string constantValue;

                    switch (memberExp.Type)
                    {
                        case Type boolType when boolType == typeof(bool):
                            constantValue = Convert.ToInt32(constantExp.Value).ToString();
                            break;
                        case Type stringType when stringType == typeof(string):
                            constantValue = $"'{constantExp.Value}'";
                            break;
                        case Type intType when intType == typeof(int):
                            constantValue = constantExp.ToString();
                            break;
                        default:
                            throw new NotImplementedException();
                    }


                    return $"{memberExp.Member.Name} {ExpressionTypeMapping[binExp.NodeType]} {constantValue}";
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                var temp = $"{ToSearchString(binExp.Left)} " +
                           $"{ExpressionTypeMapping[binExp.NodeType]} " +
                           $"{ToSearchString(binExp.Right)}";

                return temp;
            }
        }
    }

}
