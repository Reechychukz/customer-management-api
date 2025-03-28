﻿using System.Linq.Expressions;

namespace Application.Helpers
{
    public static class SortHelper<T>
    {
        public static IEnumerable<T> SortT(IEnumerable<T> sortList, string parameter)
        {
            var isAscending = GetIsAscending(parameter);

            string param = string.Empty;
            if (parameter.Length > 0)
            {
                param = parameter[1..];
            }
            else
            {
                return null;
            }

            var pi = typeof(T).GetProperty(param);

            if (pi != null)
                sortList = isAscending ? sortList.OrderBy(a => pi.GetValue(a, null)) : sortList.OrderByDescending(a => pi.GetValue(a, null));

            return sortList;
        }

        public static IQueryable<T> OrderByDynamic(IQueryable<T> query, string attribute)
        {
            try
            {
                var isAscending = GetIsAscending(attribute);
                string parameter = string.Empty;
                if (attribute.Length > 0)
                {
                    parameter = attribute[1..];
                }
                else
                {
                    return query;
                }
                string orderMethodName = isAscending ? "OrderBy" : "OrderByDescending";
                Type t = typeof(T);

                var param = Expression.Parameter(t, parameter);
                var property = t.GetProperty(parameter);

                return query.Provider.CreateQuery<T>(
                    Expression.Call(
                        typeof(Queryable),
                        orderMethodName,
                        new Type[] { t, property.PropertyType },
                        query.Expression,
                        Expression.Quote(
                            Expression.Lambda(
                                Expression.Property(param, property),
                                param))
                    ));
            }
            catch (Exception)
            {
                return query;
            }
        }

        private static bool GetIsAscending(string parameter)
        {
            if (string.IsNullOrEmpty(parameter))
                return false;
            char sign = parameter[0];

            if (sign == '-')
                return false;
            else
                return true;
        }
    }
}

