using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Windows Update Object support Operator
        /// ref: https://docs.microsoft.com/en-us/windows/win32/api/wuapi/nf-wuapi-iupdatesearcher-search
        /// </summary>
        private static readonly Dictionary<string, HashSet<ExpressionType>> MemberAllowType = new Dictionary<string, HashSet<ExpressionType>>(StringComparer.OrdinalIgnoreCase)
        {
            {"Type",new HashSet<ExpressionType>(){ ExpressionType.Equal,ExpressionType.NotEqual} },
            {"DeploymentAction",new HashSet<ExpressionType>(){ ExpressionType.Equal} },
            {"IsAssigned",new HashSet<ExpressionType>(){ ExpressionType.Equal} },
            {"BrowseOnly",new HashSet<ExpressionType>(){ ExpressionType.Equal} },
            {"AutoSelectOnWebSites",new HashSet<ExpressionType>(){ ExpressionType.Equal} },
            {"UpdateID",new HashSet<ExpressionType>(){ ExpressionType.Equal,ExpressionType.NotEqual} },
            {"RevisionNumber",new HashSet<ExpressionType>(){ ExpressionType.Equal} },
            {"CategoryIDs",new HashSet<ExpressionType>(){ } }, //Container
            {"IsInstalled", new HashSet<ExpressionType>(){ExpressionType.Equal,ExpressionType.NotEqual} },
            {"IsHidden",new HashSet<ExpressionType>(){ ExpressionType.Equal} },
            {"IsPresent",new HashSet<ExpressionType>(){ ExpressionType.Equal} },
            {"RebootRequired",new HashSet<ExpressionType>(){ ExpressionType.Equal} },

        };

        public static ISearchResult Where(this IUpdateSearcher searcher, Expression<Predicate<ISearchObject>> expression)
        {
            var searchString = ToSearchString(expression.Body, true);
            var result = searcher.Search(searchString);
            return result;
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

                case MethodCallExpression methodCallExp:
                    return $"{ExtractMethodCallExpression(methodCallExp)}";
                default:
                    throw new NotImplementedException("Not Support Expression");
            }
        }

        private static string ExtractMethodCallExpression(MethodCallExpression methodCallExp)
        {
            var memberName = methodCallExp.Object.ToString().Split('.').Last();
            // For now , only support attribute 'CategoryIDs' and Method 'Contains'
            if (methodCallExp.Method.Name != "Contains" ||
                memberName != "CategoryIDs")
                throw new ArgumentException("Only support method 'Contains' on 'CategoryIDs'");

            return $"{memberName} contains '{(methodCallExp.Arguments[0] as ConstantExpression).Value}'";
        }

        private static string ExtractUnaryExpression(UnaryExpression unaryExp)
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
                    return HandleEnumBinaryExpression(binExp);
                }
            }
            else
            {
                var isOr = binExp.NodeType is ExpressionType.Or ||
                               binExp.NodeType is ExpressionType.OrElse;

                if (!isRoot && isOr)
                    throw new ArgumentException($"OR can be used only at the top level of the search criteria");


                var temp = $"({ToSearchString(binExp.Left, isOr)}) " +
                           $"{ExpressionTypeMapping[binExp.NodeType]} " +
                           $"({ToSearchString(binExp.Right, isOr)})";

                return temp;
            }
        }

        private static string HandleEnumBinaryExpression(BinaryExpression binExp)
        {
            if (binExp.Left is UnaryExpression unaryExp)
            {
                if (unaryExp.NodeType is ExpressionType.Convert &&
                    unaryExp.Operand is MemberExpression memberExp)
                {
                    var memberName = memberExp.Member.Name;
                    CheckMemberCanBeSearch(memberName);
                    CheckMemberAllowType(memberName, binExp.NodeType);

                    var value = Enum.Parse(memberExp.Type,
                                           $"{(binExp.Right as ConstantExpression).Value}");
                    return $"{memberName} = '{value}'";
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                throw new NotImplementedException();
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
                throw new ArgumentException($"Not support to serach attribube : '{memberName}'");

        }

        private static void CheckMemberAllowType(string memberName, ExpressionType type)
        {
            if (!MemberAllowType[memberName].Contains(type))
                throw new ArgumentException($"The attribute: '{memberName}' " +
                                                $"only support expression:{string.Join(",", MemberAllowType[memberName])}");

        }
    }

}
