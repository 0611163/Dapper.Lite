using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dapper.Lite
{
    /// <summary>
    /// 表达式树解析
    /// </summary>
    internal class ExpressionHelper<T>
    {
        #region 变量
        private IProvider _provider;
        private HashSet<string> _dbParameterNames;
        private SqlStringMethod _SqlStringMethod;

        internal string Alias { get; set; }
        #endregion

        #region 构造函数
        public ExpressionHelper(IProvider provider, HashSet<string> dbParameterNames, SqlStringMethod sqlStringMethod)
        {
            _provider = provider;
            _dbParameterNames = dbParameterNames;
            _SqlStringMethod = sqlStringMethod;
        }
        #endregion

        #region VisitLambda
        /// <summary>
        /// VisitLambda
        /// </summary>
        public string VisitLambda(Expression exp, out DbParameter[] dbParameters)
        {
            LambdaExpression lambdaExp = exp as LambdaExpression;
            if (lambdaExp.Body is UnaryExpression)
            {
                UnaryExpression unaryExp = lambdaExp.Body as UnaryExpression;

                ExpValue expValue = VisitConditions(unaryExp.Operand);

                dbParameters = expValue.DbParameters.ToArray();
                return expValue.Sql;
            }
            else if (lambdaExp.Body is MemberExpression)
            {
                MemberExpression MemberExp = lambdaExp.Body as MemberExpression;

                ExpValue expValue = VisitConditions(MemberExp);

                dbParameters = expValue.DbParameters.ToArray();
                return expValue.Sql;
            }
            else
            {
                throw new Exception("不支持");
            }
        }
        #endregion

        #region VisitConditions 访问查询条件(可能是单个查询条件或多个查询条件)
        /// <summary>
        /// 访问查询条件(可能是单个查询条件或多个查询条件)
        /// </summary>
        public ExpValue VisitConditions(Expression exp)
        {
            ExpValue result = new ExpValue();

            if (exp.NodeType == ExpressionType.AndAlso ||
                exp.NodeType == ExpressionType.And ||
                exp.NodeType == ExpressionType.OrElse ||
                exp.NodeType == ExpressionType.Or)
            {
                BinaryExpression binaryExp = exp as BinaryExpression;
                result = VisitBinaryConditionArray(binaryExp);
            }
            else
            {
                result = VisitCondition(exp);
            }

            return result;
        }
        #endregion

        #region VisitCondition 访问单个查询条件
        /// <summary>
        /// 访问单个查询条件
        /// </summary>
        public ExpValue VisitCondition(Expression exp)
        {
            ExpValue result = new ExpValue();

            if (exp.NodeType == ExpressionType.Call) // 例: t => t.Remark.Contains("订单")
            {
                result = VisitMethodCall(exp as MethodCallExpression);
            }
            else if (exp.NodeType == ExpressionType.MemberAccess) // 支持 order by
            {
                ExpValue expValue = VisitMember(exp as MemberExpression, null);
                result.Sql = string.Format("{0}.{1}", expValue.MemberParentName, expValue.MemberDBField);
            }
            else if (exp.NodeType == ExpressionType.NotEqual ||
                 exp.NodeType == ExpressionType.GreaterThan ||
                 exp.NodeType == ExpressionType.GreaterThanOrEqual ||
                 exp.NodeType == ExpressionType.LessThan ||
                 exp.NodeType == ExpressionType.LessThanOrEqual ||
                 exp.NodeType == ExpressionType.Equal)
            {
                result = VisitBinaryCondition(exp as BinaryExpression); // 例: t => t.Status == 0 例: t => t.OrderTime >= new DateTime(2020,1,1)
            }
            else if (exp.NodeType == ExpressionType.Not) //支持 not in
            {
                UnaryExpression unaryExp = exp as UnaryExpression;
                if (unaryExp.Operand is MethodCallExpression)
                {
                    result = VisitMethodCall(unaryExp.Operand as MethodCallExpression, exp);
                }
                else
                {
                    throw new Exception("不支持");
                }
            }
            else
            {
                throw new Exception("不支持");
            }

            return result;
        }
        #endregion

        #region VisitBinaryConditionArray 访问多个查询条件
        /// <summary>
        /// 访问多个查询条件 
        /// </summary>
        public ExpValue VisitBinaryConditionArray(BinaryExpression exp) //例：t.`remark` like @Remark AND t.`create_time` < @CreateTime 
        {
            ExpValue result = new ExpValue();

            ExpValue left = VisitConditions(exp.Left);
            ExpValue right = VisitConditions(exp.Right);

            var sqlOperator = ToSqlOperator(exp.NodeType);
            if (sqlOperator == "AND")
            {
                result.Sql = string.Format(" {0} {1} {2} ", left.Sql, sqlOperator, right.Sql);
            }
            else
            {
                result.Sql = string.Format(" ({0} {1} {2}) ", left.Sql, sqlOperator, right.Sql);
            }
            result.Type = ExpValueType.SqlAndDbParameter;

            result.DbParameters.AddRange(left.DbParameters);
            result.DbParameters.AddRange(right.DbParameters);

            return result;
        }
        #endregion

        #region VisitBinaryCondition 访问单个查询条件
        /// <summary>
        /// 访问单个查询条件 
        /// </summary>
        public ExpValue VisitBinaryCondition(BinaryExpression exp) //例：t.`status` = @Status 
        {
            ExpValue result = new ExpValue();

            if (_SqlStringMethod == SqlStringMethod.LeftJoin)
            {
                ExpValue left = VisitMember(exp.Left);
                ExpValue right = VisitMember(exp.Right);

                result.Sql = string.Format("{0}.{1} = {2}.{3}", left.MemberParentName, left.MemberDBField, right.MemberParentName, right.MemberDBField);
                result.Type = ExpValueType.SqlAndDbParameter;
            }
            else
            {
                if (exp.NodeType == ExpressionType.Not ||
                    exp.NodeType == ExpressionType.NotEqual ||
                    exp.NodeType == ExpressionType.GreaterThan ||
                    exp.NodeType == ExpressionType.GreaterThanOrEqual ||
                    exp.NodeType == ExpressionType.LessThan ||
                    exp.NodeType == ExpressionType.LessThanOrEqual ||
                    exp.NodeType == ExpressionType.Equal)
                {
                    ExpValue left;
                    ExpValue right;
                    var isrRversed = false;
                    if (IsMember(exp.Left))
                    {
                        left = VisitMember(exp.Left);
                        right = VisitValue(exp.Right);
                    }
                    else
                    {
                        left = VisitMember(exp.Right);
                        right = VisitValue(exp.Left);
                        isrRversed = true;
                    }

                    left.MemberAliasName = GetAliasName(left.MemberAliasName);
                    _dbParameterNames.Add(left.MemberAliasName);

                    if (right.Value == null)
                    {
                        if (exp.NodeType == ExpressionType.Not ||
                            exp.NodeType == ExpressionType.NotEqual)
                        {
                            result.Sql = string.Format(" {0}.{1} is not null ", left.MemberParentName, left.MemberDBField);
                        }
                        else if (exp.NodeType == ExpressionType.Equal)
                        {
                            result.Sql = string.Format(" {0}.{1} is null ", left.MemberParentName, left.MemberDBField);
                        }
                        else
                        {
                            throw new Exception("不支持");
                        }
                    }
                    else
                    {
                        if (right.Value.GetType() == typeof(DateTime))
                        {
                            SqlValue sqlValue = new SqlValue(right.Value);
                            Type parameterType = sqlValue.Value == null ? typeof(object) : sqlValue.Value.GetType();
                            string markKey = _provider.GetParameterName(left.MemberAliasName, parameterType);

                            result.DbParameters.Add(_provider.GetDbParameter(left.MemberAliasName, right.Value));
                            result.Sql = string.Format(" {0}.{1} {2} {3} ", left.MemberParentName, left.MemberDBField, ToSqlOperator(exp.NodeType, isrRversed), sqlValue.Sql.Replace("{0}", markKey));
                        }
                        else
                        {
                            string markKey = _provider.GetParameterName(left.MemberAliasName, right.Value.GetType());
                            result.DbParameters.Add(_provider.GetDbParameter(left.MemberAliasName, right.Value));
                            result.Sql = string.Format(" {0}.{1} {2} {3} ", left.MemberParentName, left.MemberDBField, ToSqlOperator(exp.NodeType, isrRversed), markKey);
                        }
                    }
                }
            }

            return result;
        }
        #endregion

        #region VisitMethodCall 方法
        /// <summary>
        /// 方法
        /// </summary>
        public ExpValue VisitMethodCall(MethodCallExpression exp, Expression parent = null)
        {
            ExpValue result = new ExpValue();

            if (exp.Method.Name == "Contains" ||
                exp.Method.Name == "StartsWith" ||
                exp.Method.Name == "EndsWith")
            {
                if (exp.Object is MemberExpression
                    && (exp.Object as MemberExpression).Type.Name != typeof(List<>).Name)
                {
                    SqlValue sqlValue = null;
                    if (exp.Method.Name == "Contains") sqlValue = new SqlValue("%" + InvokeValue(exp.Arguments[0]).ToString() + "%");
                    if (exp.Method.Name == "StartsWith") sqlValue = new SqlValue(InvokeValue(exp.Arguments[0]).ToString() + "%");
                    if (exp.Method.Name == "EndsWith") sqlValue = new SqlValue("%" + InvokeValue(exp.Arguments[0]).ToString());
                    ExpValue expValue = VisitMember(exp.Object as MemberExpression, null);

                    expValue.MemberAliasName = GetAliasName(expValue.MemberAliasName);
                    _dbParameterNames.Add(expValue.MemberAliasName);

                    Type parameterType = sqlValue.Value.GetType();
                    string markKey = _provider.GetParameterName(expValue.MemberAliasName, parameterType);

                    string not = string.Empty;
                    if (parent != null && parent.NodeType == ExpressionType.Not) // not like
                    {
                        not = "not";
                    }

                    result.Sql = string.Format("{0}.{1} {2} like {3}", expValue.MemberParentName, expValue.MemberDBField, not, sqlValue.Sql.Replace("{0}", markKey));
                    result.DbParameters.Add(_provider.GetDbParameter(expValue.MemberAliasName, sqlValue.Value));
                }
                else // 支持 in 和 not in 例: t => idList.Contains(t.Id)
                {
                    if (exp.Method.Name == "Contains")
                    {
                        SqlValue sqlValue = null;
                        ExpValue expValue = null;
                        if (exp.Object != null) //List
                        {
                            sqlValue = _provider.ForList((IList)InvokeValue(exp.Object));

                            expValue = VisitMember(exp.Arguments[0], null);


                        }
                        else //数组
                        {
                            sqlValue = _provider.ForList((IList)InvokeValue(exp.Arguments[0]));

                            expValue = VisitMember(exp.Arguments[1], null);
                        }

                        expValue.MemberAliasName = GetAliasName(expValue.MemberAliasName);
                        _dbParameterNames.Add(expValue.MemberAliasName);

                        Type parameterType = sqlValue.Value.GetType();
                        string markKey = _provider.GetParameterName(expValue.MemberAliasName, parameterType);

                        string inOrNotIn = string.Empty;
                        if (parent != null && parent.NodeType == ExpressionType.Not)
                        {
                            inOrNotIn = "not in";
                        }
                        else
                        {
                            inOrNotIn = "in";
                        }

                        result.Sql = string.Format("{0}.{1} {2} {3}", expValue.MemberParentName, expValue.MemberDBField, inOrNotIn, sqlValue.Sql.Replace("{0}", markKey));

                        string[] keyArr = sqlValue.Sql.Replace("(", string.Empty).Replace(")", string.Empty).Replace("@", string.Empty).Split(',');
                        IList valueList = (IList)sqlValue.Value;
                        for (int k = 0; k < valueList.Count; k++)
                        {
                            object item = valueList[k];
                            result.DbParameters.Add(_provider.GetDbParameter(keyArr[k], item));
                        }

                        //处理同名参数
                        var processedParams = ProcessParams(result.DbParameters, result.Sql);
                        result.Sql = processedParams.Item1;
                        result.DbParameters = processedParams.Item2;
                    }
                    else
                    {
                        throw new Exception("不支持");
                    }
                }
            }
            else // 支持 ToString、Parse 等其它方法
            {
                result.Value = ReflectionValue(exp, null);
                result.Type = ExpValueType.OnlyValue;
            }

            return result;
        }
        #endregion

        #region VisitValue 取值
        /// <summary>
        /// 第一级
        /// </summary>
        public ExpValue VisitValue(Expression exp, MemberExpression parent = null)
        {
            ExpValue result = new ExpValue();

            if (exp.NodeType == ExpressionType.Call) // 例: t => t.Status == int.Parse("0") 例: t => t.OrderTime <= DateTime.Now.AddDays(1)
            {
                result = VisitMethodCall(exp as MethodCallExpression);
            }
            else if (exp.NodeType == ExpressionType.New) // 例: t => t.OrderTime > new DateTime(2020, 1, 1)
            {
                result = VisitNew(exp as NewExpression);
            }
            else if (exp.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression memberExp = exp as MemberExpression;
                if (memberExp.Expression is MemberExpression) // 支持对象变量的属性 例: t => t.OrderTime > time.startTime.Value 例: t => t.Remark.Contains(order.Remark)
                {
                    result = VisitValue(memberExp.Expression, memberExp);
                }
                else
                {
                    object obj = ReflectionValue(exp, parent); // 例: t => t.OrderTime < DateTime.Now  例: t => t.Remark.Contains(new BsOrder().Remark)
                    result.Value = obj;
                    result.Type = ExpValueType.OnlyValue;
                }
            }
            else if (exp.NodeType == ExpressionType.Constant) // 支持常量、null
            {
                result.Value = VisitConstant(exp);
                result.Type = ExpValueType.OnlyValue;
            }
            else if (exp.NodeType == ExpressionType.Convert) // 字段是可空类型的情况
            {
                result = VisitConvert(exp);
            }
            else
            {
                throw new Exception("不支持");
            }

            return result;
        }
        #endregion

        #region VisitMember 字段或属性
        /// <summary>
        /// 字段或属性
        /// </summary>
        public ExpValue VisitMember(Expression exp, MemberExpression parent = null)
        {
            ExpValue result = new ExpValue();

            if (exp.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression mebmerExp = exp as MemberExpression;
                if (mebmerExp.Expression is ParameterExpression) // 例: exp = t.Remark
                {
                    ParameterExpression parameterExp = mebmerExp.Expression as ParameterExpression;

                    result.MemberParentName = parameterExp.Name;
                    result.MemberDBField = GetDbField(mebmerExp.Member.Name, mebmerExp.Expression.Type);
                    result.MemberName = mebmerExp.Member.Name;
                    result.MemberAliasName = mebmerExp.Member.Name;
                    result.Type = ExpValueType.MemberValue;

                    if (Alias == null && mebmerExp.Expression.Type == typeof(T))
                    {
                        Alias = parameterExp.Name;
                    }
                }
                else
                {
                    throw new Exception("不支持");
                }
            }
            else if (exp.NodeType == ExpressionType.Convert) //例：exp = t.OrderTime >= startTime (表达式左边OrderTime的类型是可空类型DateTime?)
            {
                return VisitMember((exp as UnaryExpression).Operand);
            }
            else
            {
                throw new Exception("不支持");
            }

            return result;
        }
        #endregion

        #region InvokeValue
        public object InvokeValue(Expression exp)
        {
            object result;

            if (exp.NodeType == ExpressionType.Constant)  //常量
            {
                result = VisitConstant(exp);
            }
            else if (exp.NodeType == ExpressionType.MemberAccess)
            {
                var memberExp = exp as MemberExpression;
                if (memberExp.Expression is ConstantExpression)
                {
                    var target = VisitConstant(memberExp.Expression);
                    result = GetMemberValue(memberExp.Member, target);
                }
                else if (memberExp.Expression is NewExpression newExp)
                {
                    var target = VisitNew(newExp).Value;
                    result = GetMemberValue(memberExp.Member, target);
                }
                else if (memberExp.Type == typeof(DateTime))
                {
                    if (memberExp.Expression == null)
                    {
                        result = GetMemberValue(memberExp.Member, null);
                    }
                    else
                    {
                        var target = InvokeValue(memberExp.Expression);
                        result = GetMemberValue(memberExp.Member, target);
                    }
                }
                else
                {
                    result = ReflectionValue(memberExp.Expression, memberExp);
                }
            }
            else if (exp is MethodCallExpression methodCallExp)
            {
                result = InvokeCall(methodCallExp);
            }
            else if (exp.NodeType == ExpressionType.Convert)
            {
                var expValue = VisitConvert(exp);
                result = expValue.Value;
            }
            else if (exp is NewArrayExpression newArrayExp)
            {
                ArrayList arr = new ArrayList();
                foreach (var item in newArrayExp.Expressions)
                {
                    var val = InvokeValue(item);
                    arr.Add(val);
                }
                result = arr;
            }
            else if (exp is ListInitExpression)
            {
                ArrayList arr = new ArrayList();
                if (exp.CanReduce)
                {
                    var blockExpression = exp.Reduce() as BlockExpression;
                    var expressions = blockExpression.Expressions.Skip(1).Take(blockExpression.Expressions.Count - 2).ToList();
                    for (int i = 0; i < expressions.Count; i++)
                    {
                        var methodCallExpression = expressions[i] as MethodCallExpression;
                        foreach (var item in methodCallExpression.Arguments)
                        {
                            var val = InvokeValue(item);
                            arr.Add(val);
                        }
                    }
                }
                result = arr;
            }
            else if (exp is NewExpression newExp)
            {
                var expValue = VisitNew(newExp);
                result = expValue.Value;
            }
            else
            {
                result = Expression.Lambda(exp).Compile().DynamicInvoke();
            }

            return result;
        }
        #endregion

        #region ReflectionValue
        private object ReflectionValue(Expression member, MemberExpression parent)
        {
            object result;

            if (member == null)
            {
                if (parent.Type == typeof(DateTime))
                {
                    result = GetMemberValue(parent.Member, null);
                }
                else
                {
                    result = Expression.Lambda(parent).Compile().DynamicInvoke();
                }
            }
            else if (parent != null && parent.NodeType == ExpressionType.MemberAccess)
            {
                if (parent.Expression is MemberExpression)
                {
                    var target = ReflectionValue(parent.Expression, null);
                    result = GetMemberValue(parent.Member, target);
                }
                else if (parent.Expression is ConstantExpression)
                {
                    var target = VisitConstant(parent.Expression);
                    result = GetMemberValue(parent.Member, target);
                }
                else
                {
                    result = Expression.Lambda(member).Compile().DynamicInvoke();
                }
            }
            else if (parent == null && member.NodeType == ExpressionType.MemberAccess)
            {
                var memberExp = member as MemberExpression;
                if (memberExp.Expression is ConstantExpression)
                {
                    var target = VisitConstant(memberExp.Expression);
                    result = GetMemberValue(memberExp.Member, target);
                }
                else if (memberExp.Expression is NewExpression newExp)
                {
                    var expValue = VisitNew(newExp);
                    result = GetMemberValue(memberExp.Member, expValue.Value);
                }
                else if (memberExp.Type == typeof(DateTime))
                {
                    result = GetMemberValue(memberExp.Member, null);
                }
                else
                {
                    result = Expression.Lambda(member).Compile().DynamicInvoke();
                }
            }
            else if (parent == null && member.NodeType == ExpressionType.Call)
            {
                result = InvokeCall(member as MethodCallExpression);
            }
            else if (parent == null && member.NodeType == ExpressionType.Convert)
            {
                var expValue = VisitConvert(member);
                result = expValue.Value;
            }
            else
            {
                result = Expression.Lambda(member).Compile().DynamicInvoke();

                if (result != null && result.GetType().IsClass && result.GetType() != typeof(string) && parent != null)
                {
                    result = Expression.Lambda(parent).Compile().DynamicInvoke();
                }
            }

            return result;
        }
        #endregion

        #region InvokeCall
        private object InvokeCall(MethodCallExpression exp)
        {
            var arguments = new List<object>();

            foreach (var item in exp.Arguments)
            {
                arguments.Add(InvokeValue(item));
            }

            if (exp.Object == null)
            {
                return exp.Method.Invoke(this, arguments.ToArray());
            }
            else
            {
                var obj = InvokeValue(exp.Object);
                return exp.Method.Invoke(obj, arguments.ToArray());
            }
        }
        #endregion

        #region VisitConstant 常量表达式
        /// <summary>
        /// 常量表达式
        /// </summary>
        private object VisitConstant(Expression exp)
        {
            if (exp is ConstantExpression)
            {
                ConstantExpression constantExp = exp as ConstantExpression;
                return constantExp.Value;
            }
            else
            {
                throw new Exception("不是ConstantExpression");
            }
        }
        #endregion

        #region VisitNew
        /// <summary>
        /// New 表达式
        /// </summary>
        public ExpValue VisitNew(NewExpression exp)
        {
            ExpValue result = new ExpValue();

            List<object> args = new List<object>();
            foreach (Expression argExp in exp.Arguments.ToArray())
            {
                args.Add(InvokeValue(argExp));
            }

            result.Value = exp.Constructor.Invoke(args.ToArray());
            result.Type = ExpValueType.OnlyValue;

            return result;
        }
        #endregion

        #region VisitConvert
        /// <summary>
        /// Convert 表达式
        /// </summary>
        public ExpValue VisitConvert(Expression exp)
        {
            ExpValue result = new ExpValue();

            Expression operandExp = (exp as UnaryExpression).Operand;
            if (operandExp is UnaryExpression)
            {
                result = VisitValue((operandExp as UnaryExpression).Operand);
            }
            else if (operandExp is MemberExpression)
            {
                result = VisitValue(operandExp);
            }
            else if (operandExp is ConstantExpression)
            {
                result.Value = VisitConstant(operandExp);
                result.Type = ExpValueType.OnlyValue;
            }
            else if (operandExp is NewExpression)
            {
                result = VisitNew(operandExp as NewExpression);
            }
            else if (operandExp is MethodCallExpression)
            {
                result = VisitValue(operandExp);
            }
            else
            {
                throw new Exception("不支持");
            }

            return result;
        }
        #endregion

        #region ToSqlOperator
        private string ToSqlOperator(ExpressionType type, bool isReversed = false)
        {
            if (!isReversed)
            {
                switch (type)
                {
                    case (ExpressionType.AndAlso):
                    case (ExpressionType.And):
                        return "AND";
                    case (ExpressionType.OrElse):
                    case (ExpressionType.Or):
                        return "OR";
                    case (ExpressionType.Not):
                        return "NOT";
                    case (ExpressionType.NotEqual):
                        return "<>";
                    case ExpressionType.GreaterThan:
                        return ">";
                    case ExpressionType.GreaterThanOrEqual:
                        return ">=";
                    case ExpressionType.LessThan:
                        return "<";
                    case ExpressionType.LessThanOrEqual:
                        return "<=";
                    case (ExpressionType.Equal):
                        return "=";
                    default:
                        throw new Exception("不支持该方法");
                }
            }
            else
            {
                switch (type)
                {
                    case (ExpressionType.AndAlso):
                    case (ExpressionType.And):
                        return "AND";
                    case (ExpressionType.OrElse):
                    case (ExpressionType.Or):
                        return "OR";
                    case (ExpressionType.Not):
                        return "NOT";
                    case (ExpressionType.NotEqual):
                        return "<>";
                    case ExpressionType.GreaterThan:
                        return "<";
                    case ExpressionType.GreaterThanOrEqual:
                        return "<=";
                    case ExpressionType.LessThan:
                        return ">";
                    case ExpressionType.LessThanOrEqual:
                        return ">=";
                    case (ExpressionType.Equal):
                        return "=";
                    default:
                        throw new Exception("不支持该方法");
                }
            }
        }
        #endregion

        #region GetDbField
        private string GetDbField(string name, Type type)
        {
            var dict = DbSession.GetEntityPropertiesDict(type);
            if (dict.TryGetValue(name, out PropertyInfoEx propInfoEx))
            {
                PropertyInfo propertyInfo = propInfoEx.PropertyInfo;

                ColumnAttribute dbFieldAttribute = propInfoEx.DBFieldAttribute;
                if (dbFieldAttribute != null && dbFieldAttribute.Name != null)
                {
                    return _provider.OpenQuote + dbFieldAttribute.Name + _provider.CloseQuote;
                }
                else
                {
                    return _provider.OpenQuote + propertyInfo.Name + _provider.CloseQuote;
                }
            }
            else
            {
                throw new Exception($"GetDbField错误, name={name}, type={type.Name}");
            }
        }
        #endregion

        #region GetAliasName
        /// <summary>
        /// 获取不冲突的别名
        /// </summary>
        private string GetAliasName(string aliasName)
        {
            int index = 0;
            while (_dbParameterNames.Contains(aliasName + (index == 0 ? "" : index.ToString())))
            {
                index++;
            }
            aliasName += (index == 0 ? "" : index.ToString());
            _dbParameterNames.Add(aliasName);
            return aliasName;
        }
        #endregion

        #region ProcessParams
        /// <summary>
        /// 处理同名参数
        /// </summary>
        protected Tuple<string, List<DbParameter>> ProcessParams(List<DbParameter> cmdParams, string sql)
        {
            List<DbParameter> newParamList = new List<DbParameter>();
            foreach (DbParameter param in cmdParams)
            {
                if (!_dbParameterNames.Contains(param.ParameterName))
                {
                    _dbParameterNames.Add(param.ParameterName);
                    newParamList.Add(param);
                }
                else
                {
                    int index = 0;
                    while (_dbParameterNames.Contains(param.ParameterName + (index == 0 ? "" : index.ToString())))
                    {
                        index++;
                    }
                    string newName = param.ParameterName + (index == 0 ? "" : index.ToString());
                    DbParameter newParam = _provider.GetDbParameter(newName, param.Value);
                    _dbParameterNames.Add(newParam.ParameterName);
                    newParamList.Add(newParam);
                    string oldParamName = _provider.GetParameterName(param.ParameterName, param.Value.GetType());
                    string newParamName = _provider.GetParameterName(newParam.ParameterName, param.Value.GetType());
                    int pos = sql.IndexOf(oldParamName);
                    Regex regex = new Regex(oldParamName + "[)]{1}", RegexOptions.None);
                    Regex regex2 = new Regex(oldParamName + "[\\s]{1}", RegexOptions.None);
                    Regex regex3 = new Regex(oldParamName + "[,]{1}", RegexOptions.None);
                    if (regex.IsMatch(sql))
                    {
                        sql = regex.Replace(sql, newParamName + ")", 1);
                    }
                    else if (regex2.IsMatch(sql))
                    {
                        sql = regex2.Replace(sql, newParamName + " ", 1);
                    }
                    else
                    {
                        sql = regex3.Replace(sql, newParamName + ",", 1);
                    }
                }
            }
            return new Tuple<string, List<DbParameter>>(sql, newParamList);
        }
        #endregion

        #region GetMemberValue
        private object GetMemberValue(MemberInfo propertyOrField, object target)
        {
            if (propertyOrField is PropertyInfo propertyInfo)
            {
                return propertyInfo.GetValue(target, null);
            }
            else if (propertyOrField is FieldInfo fieldInfo)
            {
                return fieldInfo.GetValue(target);
            }
            else
            {
                throw new ArgumentOutOfRangeException("propertyOrField", "Expected a property or field, not " + propertyOrField);
            }
        }
        #endregion

        #region IsMember
        private bool IsMember(Expression exp)
        {
            if (exp is MemberExpression memberExp)
            {
                if (memberExp.Expression is ParameterExpression)
                {
                    return true;
                }
            }
            else if (exp.NodeType == ExpressionType.Convert)
            {
                if (exp is UnaryExpression unaryExp)
                {
                    return IsMember(unaryExp.Operand);
                }
            }
            return false;
        }
        #endregion

    }
}
