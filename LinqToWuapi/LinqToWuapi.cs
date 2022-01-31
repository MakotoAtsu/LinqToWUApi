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

        public static IUpdate Where(this IUpdateSearcher searcher, Expression<Predicate<IUpdate4>> expression)
        {


            throw new NotImplementedException();
        }

        public static string ToSearchString(Expression expression,
                                            bool isRoot = false)
        {

            switch (expression)
            {
                case BinaryExpression binExp:
                    return $"{ExtractBinaryExpression(binExp, isRoot)}";

                case MemberExpression memberExp:
                    return $"{ExtractMemberExpression(memberExp)}";

                case UnaryExpression unaryExp:
                    return $"{ExtractUnaryExpression(unaryExp)}";
                default:
                    throw new NotImplementedException("Only support BinaryExpression");
            }

            throw new NotImplementedException();
        }

        private static object ExtractUnaryExpression(UnaryExpression unaryExp)
        {
            if (!(unaryExp.Operand is MemberExpression memberExp))
                throw new ArgumentException("UnaryExpression just can use with 'NOT' and 'MemberExpression'");

            var memberName = memberExp.Member.Name;
            CheckMemberCanBeSearch(memberName);

            CheckMemberAllowType(memberName, ExpressionType.Equal);

            return $"{memberName} = 0";
        }

        private static string ExtractBinaryExpression(BinaryExpression binExp,
                                                      bool isRoot)
        {

            // if Right is Constant , Left must be Member
            if (binExp.Right is ConstantExpression constantExp)
            {
                if (binExp.Left is MemberExpression memberExp)
                {
                    var memberName = memberExp.Member.Name;

                    CheckMemberCanBeSearch(memberName);

                    CheckMemberAllowType(memberName, binExp.NodeType);

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
                //if (!isRoot &&
                //    (binExp.NodeType == ExpressionType.Or || binExp.NodeType == ExpressionType.OrElse))
                //    throw new ArgumentException($"OR can be used only at the top level of the search criteria");

                var temp = $"({ToSearchString(binExp.Left)}) " +
                           $"{ExpressionTypeMapping[binExp.NodeType]} " +
                           $"({ToSearchString(binExp.Right)})";

                return temp;
            }
        }


        private static string ExtractMemberExpression(MemberExpression memberExp)
        {
            var memberName = memberExp.Member.Name;

            CheckMemberCanBeSearch(memberName);

            return $"{memberName} = 1";
        }


        private static void CheckMemberCanBeSearch(string memberName)
        {
            if (!MemberAllowType.ContainsKey(memberName))
                throw new ArgumentException($"Not support serach attribube {memberName}");

        }

        private static void CheckMemberAllowType(string memberName, ExpressionType type)
        {
            if (!MemberAllowType[memberName].Contains(type))
                throw new NotSupportedException($"The attribute:{memberName} " +
                                                $"only support expression:{string.Join(",", MemberAllowType[memberName])}");

        }
    }

}
