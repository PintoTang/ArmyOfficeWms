using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Framework
{
    public static class LambdaExtensions
    {
        public static Expression<Func<T, bool>> True<T>() { return f => true; }

        public static Expression<Func<T, bool>> False<T>() { return f => false; }


        public static Expression<Func<T, bool>> AndAlso<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            if (first.IsEmpty())
            {
                return second;
            }
            return first.AndAlso<T>(second, Expression.AndAlso);
        }

        public static Expression<Func<T, bool>> OrElse<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            if (first.IsEmpty())
            {
                return second;
            }
            return first.AndAlso<T>(second, Expression.OrElse);
        }

        public static Expression<Func<T, T>> ToUpdateLambda<T>(this T item)
        {
            Type t = typeof(T);
            if (t.GetCustomAttribute(typeof(SugarTable), false) == null)
            {
                throw new System.Exception("当前类未定义自定义属性");
            }
            ParameterExpression parameterExp = Expression.Parameter(t, "x");
            PropertyInfo[] propertyList = t.GetProperties();
            List<MemberAssignment> memberList = new List<MemberAssignment>();
            foreach (PropertyInfo property in propertyList)
            {
                SugarColumn col = property.GetCustomAttribute<SugarColumn>();
                if (col != null)
                {
                    if (col.IsIgnore || col.IsPrimaryKey)
                    {
                        continue;
                    }
                }
                if (property.Name.ToLower() == "id")
                {
                    continue;
                }
                object fieldValue = property.GetValue(item);
                if (fieldValue == null)
                {
                    continue;
                }
                MemberAssignment memberAssignment = Expression.Bind(property, Expression.Constant(fieldValue, property.PropertyType));
                memberList.Add(memberAssignment);
            }
            if (memberList == null || memberList.Count == 0)
            {
                throw new System.Exception("未找到赋值字段");
            }
            var newExpr = Expression.New(t);
            var init = Expression.MemberInit(newExpr, memberList);
            Expression<Func<T, T>> expr = (Expression<Func<T, T>>)Expression.Lambda(init, parameterExp);
            return expr;
        }

        public static Expression<Func<T, T>> ToUpdateLambda<C, T>(this C item)
        {
            Type descT = typeof(T);
            if (descT.GetCustomAttribute(typeof(SugarTable), false) == null)
            {
                throw new System.Exception("当前类未定义自定义属性");
            }

            Type srcT = item.GetType();
            PropertyInfo[] propertyList = srcT.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            List<PropertyInfo> descPropertys = descT.GetProperties().Where(p => p.GetCustomAttribute<SugarColumn>() == null || (p.GetCustomAttribute<SugarColumn>() != null && p.GetCustomAttribute<SugarColumn>().IsIgnore == false)).ToList();

            ParameterExpression parameterExp = Expression.Parameter(descT, "x");
            //PropertyInfo[] propertyList = t.GetProperties();
            List<MemberAssignment> memberList = new List<MemberAssignment>();
            foreach (PropertyInfo srcProperty in propertyList)
            {
                PropertyInfo descProperty = descPropertys.FirstOrDefault(t => t.Name == srcProperty.Name);
                if (descProperty == null)
                {
                    continue;
                }
                object fieldValue = srcProperty.GetValue(item);
                if (fieldValue == null)
                {
                    continue;
                }
                SugarColumn col = descProperty.GetCustomAttribute<SugarColumn>();
                if (col != null)
                {
                    if (col.IsIgnore || col.IsPrimaryKey)
                    {
                        continue;
                    }
                }
                MemberAssignment memberAssignment = Expression.Bind(descProperty, Expression.Constant(fieldValue, descProperty.PropertyType));
                memberList.Add(memberAssignment);
            }
            if (memberList == null || memberList.Count == 0)
            {
                throw new System.Exception("未找到赋值字段");
            }
            var newExpr = Expression.New(descT);
            var init = Expression.MemberInit(newExpr, memberList);
            Expression<Func<T, T>> expr = (Expression<Func<T, T>>)Expression.Lambda(init, parameterExp);
            return expr;
        }

        public static Expression<Func<T, T>> ToInsertLambda<T>(this T item)
        {
            Type t = typeof(T);
            if (t.GetCustomAttribute(typeof(SugarTable), false) == null)
            {
                throw new System.Exception("当前类未定义自定义属性");
            }
            ParameterExpression parameterExp = Expression.Parameter(t, "x");
            PropertyInfo[] propertyList = t.GetProperties();
            List<MemberAssignment> memberList = new List<MemberAssignment>();
            foreach (PropertyInfo property in propertyList)
            {
                SugarColumn col = property.GetCustomAttribute<SugarColumn>();
                if (col != null)
                {
                    if (col.IsIgnore)
                    {
                        continue;
                    }
                }

                object fieldValue = property.GetValue(item);
                if (col != null)
                {
                    if (col.IsPrimaryKey)
                    {
                        if (!col.IsIdentity)
                        {
                            if (property.PropertyType == typeof(long) || property.PropertyType == typeof(long?)
                                || property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
                            {
                                if (fieldValue == null || Convert.ToInt64(fieldValue) == 0)
                                {
                                    //fieldValue = Snowflake.NewId();
                                }
                            }
                            else if (property.PropertyType == typeof(Guid) || property.PropertyType == typeof(Guid?))
                            {
                                if (fieldValue == null || Convert.ToInt32(fieldValue) == 0)
                                {
                                    fieldValue = Guid.NewGuid();
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                if (fieldValue == null)
                {
                    continue;
                }
                MemberAssignment memberAssignment = Expression.Bind(property, Expression.Constant(fieldValue, property.PropertyType));
                memberList.Add(memberAssignment);
            }
            if (memberList == null || memberList.Count == 0)
            {
                throw new System.Exception("未找到赋值字段");
            }
            var newExpr = Expression.New(t);
            var init = Expression.MemberInit(newExpr, memberList);
            Expression<Func<T, T>> expr = (Expression<Func<T, T>>)Expression.Lambda(init);
            return expr;
        }
        public static Expression<Func<T, bool>> FilterWhereLambda<C, T>(this C item, bool isValid = false)
        {
            if (item == null)
            {
                return True<T>();
            }
            Type srcT = item.GetType();
            PropertyInfo[] propertyList = srcT.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

            Type descT = typeof(T);

            ParameterExpression parameterExp = Expression.Parameter(descT, "x");
            List<PropertyInfo> descPropertys = descT.GetProperties().Where(p => p.GetCustomAttribute<SugarColumn>() == null || (p.GetCustomAttribute<SugarColumn>() != null && p.GetCustomAttribute<SugarColumn>().IsIgnore == false)).ToList();
            PropertyInfo descKeyProperty = descPropertys.FirstOrDefault(p => p.GetCustomAttribute<SugarColumn>() != null && p.GetCustomAttribute<SugarColumn>().IsPrimaryKey);
            if (descKeyProperty == null)
            {
                throw new System.Exception("未找到主键");
            }

            Expression totalExpr = null;// Expression.Constant(true);
            foreach (PropertyInfo property in propertyList)
            {
                PropertyInfo descProperty = descPropertys.FirstOrDefault(t => t.Name == property.Name);
                if (descProperty == null)
                {
                    continue;
                }
                object fieldValue = property.GetValue(item);
                if (fieldValue == null)
                {
                    continue;
                }
                Expression left = Expression.Property(parameterExp, descProperty);
                Expression right = Expression.Constant(fieldValue, descProperty.PropertyType);
                if (descProperty == descKeyProperty)
                {
                    if (isValid)
                    {
                        if (totalExpr == null)
                        {
                            totalExpr = Expression.NotEqual(left, right);
                        }
                        else
                        {
                            totalExpr = Expression.AndAlso(totalExpr, Expression.NotEqual(left, right));
                        }
                        continue;
                    }
                }
                Expression filter = Expression.Equal(left, right);
                if (totalExpr == null)
                {
                    totalExpr = filter;
                }
                else
                {
                    totalExpr = Expression.AndAlso(totalExpr, filter);
                }

            }
            if (totalExpr == null)
            {
                totalExpr = Expression.Constant(true);
            }
            Expression<Func<T, bool>> expr = (Expression<Func<T, bool>>)Expression.Lambda(totalExpr, parameterExp);
            return expr;
        }

        public static Expression<Func<T, bool>> FilterWhereLambda<C, T>(this C item, string key, Func<string, Expression<Func<T, bool>>> callback)
        {
            Expression<Func<T, bool>> expr = item.FilterWhereLambda<C, T>();
            if (key.NotNull())
            {
                if (callback != null)
                {
                    Expression<Func<T, bool>> ext2 = callback(key);
                    if (!ext2.IsEmpty())
                    {
                        expr = expr.AndAlso(ext2);
                    }
                }
            }
            return expr;
        }
        public static Expression<Func<T, bool>> FilterWhereLambda<C, T>(this C item, Expression<Func<T, bool>> where, string key, Func<string, Expression<Func<T, bool>>> callback)
        {
            Expression<Func<T, bool>> expr = item.FilterWhereLambda<C, T>();
            if (key.NotNull())
            {
                if (callback != null)
                {
                    Expression<Func<T, bool>> ext2 = callback(key);
                    if (!ext2.IsEmpty())
                    {
                        expr = expr.AndAlso(ext2);
                    }
                }
            }
            if (!where.IsEmpty())
            {
                expr = expr.AndAlso(where);
            }
            return expr;
        }

        private static Expression<Func<T, bool>> AndAlso<T>(
        this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2,
        Func<Expression, Expression, BinaryExpression> func)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(
                func(left, right), parameter);
        }

        #region Linq表达式判断
        /// <summary>
        /// 是否是空表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool IsEmpty<T>(this Expression<Func<T, bool>> ext)
        {
            if (ext == null)
            {
                return true;
            }
            return ext.Body.ToString() == "True" || string.IsNullOrEmpty(ext.ToString());
        }
        /// <summary>
        /// 是否不是空表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool NotEmpty<T>(this Expression<Func<T, bool>> ext)
        {
            return !IsEmpty(ext);
        }

        /// <summary>
        /// 是否是空表达式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool IsEmpty<T1, T2>(this Expression<Func<T1, T2, bool>> ext)
        {
            if (ext == null)
            {
                return true;
            }
            return ext.Body.ToString() == "True" || string.IsNullOrEmpty(ext.ToString());
        }
        /// <summary>
        /// 是否不是空表达式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool NotEmpty<T1, T2>(this Expression<Func<T1, T2, bool>> ext)
        {
            return !IsEmpty(ext);
        }

        /// <summary>
        /// 是否是空表达式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool IsEmpty<T1, T2, T3>(this Expression<Func<T1, T2, T3, bool>> ext)
        {
            if (ext == null)
            {
                return true;
            }
            return ext.Body.ToString() == "True" || string.IsNullOrEmpty(ext.ToString());
        }
        /// <summary>
        /// 是否不是空表达式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool NotEmpty<T1, T2, T3>(this Expression<Func<T1, T2, T3, bool>> ext)
        {
            return !IsEmpty(ext);
        }
        /// <summary>
        /// 是否是空表达式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool IsEmpty<T1, T2, T3, T4>(this Expression<Func<T1, T2, T3, T4, bool>> ext)
        {
            if (ext == null)
            {
                return true;
            }
            return ext.Body.ToString() == "True" || string.IsNullOrEmpty(ext.ToString());
        }
        /// <summary>
        /// 是否不是空表达式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool NotEmpty<T1, T2, T3, T4>(this Expression<Func<T1, T2, T3, T4, bool>> ext)
        {
            return !IsEmpty(ext);
        }
        /// <summary>
        /// 是否是空表达式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool IsEmpty<T1, T2, T3, T4, T5>(this Expression<Func<T1, T2, T3, T4, T5, bool>> ext)
        {
            if (ext == null)
            {
                return true;
            }
            return ext.Body.ToString() == "True" || string.IsNullOrEmpty(ext.ToString());
        }
        /// <summary>
        /// 是否是空表达式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool NotEmpty<T1, T2, T3, T4, T5>(this Expression<Func<T1, T2, T3, T4, T5, bool>> ext)
        {
            return !IsEmpty(ext);
        }

        /// <summary>
        /// 是否是空表达式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool IsEmpty<T1, T2, T3, T4, T5, T6>(this Expression<Func<T1, T2, T3, T4, T5, T6, bool>> ext)
        {
            if (ext == null)
            {
                return true;
            }
            return ext.Body.ToString() == "True" || string.IsNullOrEmpty(ext.ToString());
        }
        /// <summary>
        /// 是否不是空表达式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool NotEmpty<T1, T2, T3, T4, T5, T6>(this Expression<Func<T1, T2, T3, T4, T5, T6, bool>> ext)
        {
            return !IsEmpty(ext);
        }

        /// <summary>
        /// 是否是空表达式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool IsEmpty<T1, T2, T3, T4, T5, T6, T7>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, bool>> ext)
        {
            if (ext == null)
            {
                return true;
            }
            return ext.Body.ToString() == "True" || string.IsNullOrEmpty(ext.ToString());
        }
        /// <summary>
        /// 是否不是空表达式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool NotEmpty<T1, T2, T3, T4, T5, T6, T7>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, bool>> ext)
        {
            return !IsEmpty(ext);
        }
        /// <summary>
        /// 是否是空表达式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool IsEmpty<T1, T2, T3, T4, T5, T6, T7, T8>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, bool>> ext)
        {
            if (ext == null)
            {
                return true;
            }
            return ext.Body.ToString() == "True" || string.IsNullOrEmpty(ext.ToString());
        }
        /// <summary>
        /// 是否不是空表达式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool NotEmpty<T1, T2, T3, T4, T5, T6, T7, T8>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, bool>> ext)
        {
            return !IsEmpty(ext);
        }
        /// <summary>
        /// 是否是空表达式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool IsEmpty<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, bool>> ext)
        {
            if (ext == null)
            {
                return true;
            }
            return ext.Body.ToString() == "True" || string.IsNullOrEmpty(ext.ToString());
        }
        /// <summary>
        /// 是否不是空表达式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool NotEmpty<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, bool>> ext)
        {
            return !IsEmpty(ext);
        }
        /// <summary>
        /// 是否是空表达式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool IsEmpty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> ext)
        {
            if (ext == null)
            {
                return true;
            }
            return ext.Body.ToString() == "True" || string.IsNullOrEmpty(ext.ToString());
        }
        /// <summary>
        /// 是否不是空表达式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool NotEmpty<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> ext)
        {
            return !IsEmpty(ext);
        }
        #endregion
    }

    internal class ReplaceExpressionVisitor
            : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression node)
        {
            if (node == _oldValue)
                return _newValue;
            return base.Visit(node);
        }
    }
}
