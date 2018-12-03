using Serialize.Linq.Serializers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace ROYN
{
    public static class RoynHelper
    {
        //public static RoynResult RoynSelect<T>(this IQueryable<T> source, RoynRequest<T> roynRequest) where T : class
        //{
        //    var builder = new SelectLambdaBuilder(roynRequest.CLRType);
        //    var newSource = HandleIncludes(source.AsNoTracking(), roynRequest.Columns.ToArray());
        //    newSource = HandleFilter(newSource, roynRequest);
        //    var data = newSource.Select(builder.CreateNewStatement<T>(roynRequest.Columns.ToArray()));
        //    var orderedData = HandleOrder(newSource, roynRequest);
        //    var result = new RoynResult();
        //    var list = orderedData.ToList();
        //    result.SetResult(list, roynRequest.Columns.ToArray());
        //    return result;
        //}

        private static IQueryable<T> HandleFilter<T>(IQueryable<T> source, RoynRequest<T> roynRequest) where T : class
        {
            if (string.IsNullOrEmpty(roynRequest.InternalFilter))
            {
                return source;
            }

            var Serializer = new ExpressionSerializer(new JsonSerializer(), new Serialize.Linq.Factories.FactorySettings { AllowPrivateFieldAccess = true });
            var exp = (Expression<Func<T, bool>>)Serializer.DeserializeText(roynRequest.InternalFilter);
            return source.Where(exp);
        }

        private static IQueryable<T> HandleFilter<T>(DbQuery<T> source, RoynRequest<T> roynRequest) where T : class
        {
            if (string.IsNullOrEmpty(roynRequest.InternalFilter))
            {
                return source;
            }

            var Serializer = new ExpressionSerializer(new JsonSerializer(), new Serialize.Linq.Factories.FactorySettings { AllowPrivateFieldAccess = true });
            var exp = (Expression<Func<T, bool>>)Serializer.DeserializeText(roynRequest.InternalFilter);
            return source.Where(exp);
        }

        private static IOrderedQueryable<T> HandleOrder<T>(IQueryable<T> source, RoynRequest<T> roynRequest) where T : class
        {
            if (roynRequest.InternalOrders.Count == 0)
            {
                return source.OrderBy(roynRequest.Columns.FirstOrDefault());
            }

            var orders = roynRequest.InternalOrders.GetEnumerator();
            IOrderedQueryable<T> orderedSource = null;
            if (orders.MoveNext())
            {
                orderedSource = source.OrderBy(orders.Current.Key, orders.Current.Value);
            }

            while (orders.MoveNext())
            {
                orderedSource = orderedSource.ThenBy(orders.Current.Key, orders.Current.Value);
            }
            return orderedSource;
        }

        private static Expression<Func<T, TReturnType>> GetLambda<T, TReturnType>(IEnumerable<string> propertyNames)
        {
            var rootParameterExression = Expression.Parameter(typeof(T));

            Expression expression = rootParameterExression;
            foreach (var propertyName in propertyNames)
            {
                expression = Expression.Property(expression, propertyName);
            }
            return Expression.Lambda<Func<T, TReturnType>>(expression, rootParameterExression);
            //return NullPropagate(rootParameterExression, exp);
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> queryable, string propertyPath, SortDirection sortDirection = SortDirection.Ascending)
        {
            var propertyPathList = propertyPath.Split(Convert.ToChar("."));
            Type propertyType = typeof(T);
            foreach (var propertyName in propertyPathList)
            {
                propertyType = propertyType.GetProperty(propertyName).PropertyType;
            }

            if (propertyType == typeof(int))
            {
                var lambdaInt = GetLambda<T, int>(propertyPathList);
                return sortDirection == SortDirection.Ascending ? queryable.OrderBy((Expression<Func<T, int>>)lambdaInt) : queryable.OrderByDescending((Expression<Func<T, int>>)lambdaInt);
            }
            else if (propertyType == typeof(decimal))
            {
                var lambdaDecimal = GetLambda<T, decimal>(propertyPathList);
                return sortDirection == SortDirection.Ascending ? queryable.OrderBy((Expression<Func<T, decimal>>)lambdaDecimal) : queryable.OrderByDescending((Expression<Func<T, decimal>>)lambdaDecimal);
            }
            else if (propertyType == typeof(double))
            {
                var lambdaDouble = GetLambda<T, double>(propertyPathList);
                return sortDirection == SortDirection.Ascending ? queryable.OrderBy((Expression<Func<T, double>>)lambdaDouble) : queryable.OrderByDescending((Expression<Func<T, double>>)lambdaDouble);
            }
            else if (propertyType == typeof(DateTime))
            {
                var lambdaDatetime = GetLambda<T, DateTime>(propertyPathList);
                return sortDirection == SortDirection.Ascending ? queryable.OrderBy((Expression<Func<T, DateTime>>)lambdaDatetime) : queryable.OrderByDescending((Expression<Func<T, DateTime>>)lambdaDatetime);
            }
            else if (propertyType == typeof(int?))
            {
                var lambdaNullableInt = GetLambda<T, int?>(propertyPathList);
                return sortDirection == SortDirection.Ascending ? queryable.OrderBy((Expression<Func<T, int?>>)lambdaNullableInt) : queryable.OrderByDescending((Expression<Func<T, int?>>)lambdaNullableInt);
            }
            else if (propertyType == typeof(decimal?))
            {
                var lambdaNullableDecimal = GetLambda<T, decimal?>(propertyPathList);
                return sortDirection == SortDirection.Ascending ? queryable.OrderBy((Expression<Func<T, decimal?>>)lambdaNullableDecimal) : queryable.OrderByDescending((Expression<Func<T, decimal?>>)lambdaNullableDecimal);
            }
            else if (propertyType == typeof(DateTime?))
            {
                var lambdaNullableDatetime = GetLambda<T, DateTime?>(propertyPathList);
                return sortDirection == SortDirection.Ascending ? queryable.OrderBy((Expression<Func<T, DateTime?>>)lambdaNullableDatetime) : queryable.OrderByDescending((Expression<Func<T, DateTime?>>)lambdaNullableDatetime);
            }
            else
            if (propertyType == typeof(string))
            {
                var lambdaNullableDatetime = GetLambda<T, string>(propertyPathList);
                return sortDirection == SortDirection.Ascending ? queryable.OrderBy((Expression<Func<T, string>>)lambdaNullableDatetime) : queryable.OrderByDescending((Expression<Func<T, string>>)lambdaNullableDatetime);
            }

            var lambda = GetLambda<T, object>(propertyPathList);
            return sortDirection == SortDirection.Ascending ? queryable.OrderBy((Expression<Func<T, object>>)lambda) : queryable.OrderByDescending((Expression<Func<T, object>>)lambda);

            //MethodInfo method = typeof(RoynHelper).GetMethod(nameof(GetLambda));
            //MethodInfo generic = method.MakeGenericMethod(propertyType);
            //var l = generic.Invoke(propertyPathList, null);

            //var lambda = GetLambda<T, Decimal>(propertyPathList);

            //method = typeof(IQueryable<T>).GetMethod("OrderBy");
            //var orderByGeneric = method.MakeGenericMethod(typeof(T), propertyType);
            //method = typeof(IQueryable<T>).GetMethod("OrderByDescending");
            //var orderByDescGeneric = method.MakeGenericMethod(typeof(T), propertyType);
            //return (IOrderedEnumerable<T>)(sortDirection == SortDirection.Ascending ? orderByGeneric.Invoke(queryable, new object[] { l }) : orderByDescGeneric.Invoke(queryable, new object[] { l }));

            //return sortDirection == SortDirection.Ascending ? queryable.OrderBy(lambda) : queryable.OrderByDescending(lambda);
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> enumerable, string propertyPath, SortDirection sortDirection = SortDirection.Ascending)
        {
            var propertyPathList = propertyPath.Split(Convert.ToChar("."));
            Type propertyType = typeof(T);
            foreach (var propertyName in propertyPathList)
            {
                propertyType = propertyType.GetProperty(propertyName).PropertyType;
            }

            if (propertyType == typeof(decimal))
            {
                var lambda = GetLambda<T, decimal>(propertyPathList);
                return enumerable.OrderBy((Expression<Func<T, decimal>>)lambda);
            }
            var lamda = GetLambda<T, object>(propertyPathList);

            return sortDirection == SortDirection.Ascending ? enumerable.ThenBy((Expression<Func<T, object>>)lamda) : enumerable.ThenByDescending((Expression<Func<T, object>>)lamda);
        }

        public static IOrderedQueryable<T> OrderBy<T>(this DbQuery<T> queryable, string propertyPath)
        {
            if (queryable == null)
            {
                throw new ArgumentNullException(nameof(queryable));
            }

            var propertyPathList = propertyPath.Split(Convert.ToChar("."));
            Type propertyType = typeof(T);
            foreach (var propertyName in propertyPathList)
            {
                propertyType = propertyType.GetProperty(propertyName).PropertyType;
            }

            if (propertyType == typeof(decimal))
            {
                var lambda = GetLambda<T, decimal>(propertyPathList);
                return queryable.OrderBy((Expression<Func<T, decimal>>)lambda);
            }
            var lamda = GetLambda<T, object>(propertyPathList);
            return queryable.OrderBy((Expression<Func<T, object>>)lamda);
        }

        public static RoynResult RoynSelect<T>(this DbSet<T> source, RoynRequest<T> roynRequest) where T : class
        {
            var includeSource = HandleIncludes(source.AsNoTracking(), roynRequest.Columns.ToArray());
            var filteredSource = HandleFilter(includeSource, roynRequest);
            var orderedDataSource = HandleOrder(filteredSource, roynRequest);
            var dataSource = HandleSkipTake(orderedDataSource, roynRequest);
            // var dataSource2 = dataSource.Select(LambdaBuilder.BuildSelector<T, T>(roynRequest.Columns.ToArray()).Compile());

            var result = new RoynResult();
            result.SetResult(dataSource.ToList(), roynRequest.Columns.ToArray());

            return result;
        }

        public static RoynResult RoynSelect<T, TResult>(this DbSet<T> source, RoynRequest<T> roynRequest, RequestGraph graph) where T : class
        {
            var includeSource = HandleIncludes(source.AsNoTracking(), graph.Members.ToArray());
            var filteredSource = HandleFilter(includeSource, roynRequest);
            var orderedDataSource = HandleOrder(filteredSource, roynRequest);
            var dataSource = HandleSkipTake(orderedDataSource, roynRequest);
            var dataSource2 = dataSource.Select(LambdaBuilder.BuildSelector<T, TResult>(graph.Members));

            var result = new RoynResult();
            var ds = dataSource2.ToList();
            result.SetResult(ds, roynRequest.Columns.ToArray());

            return result;
        }

        public static RoynResult RoynSelect<T>(this DbSet<T> source, RoynRequest<T> roynRequest, RequestGraph graph) where T : class
        {
            var includeSource = HandleIncludes(source.AsNoTracking(), graph.Members.ToArray());
            var filteredSource = HandleFilter(includeSource, roynRequest);
            var orderedDataSource = HandleOrder(filteredSource, roynRequest);
            var dataSource = HandleSkipTake(orderedDataSource, roynRequest);
            var ds = dataSource.Select(LambdaBuilder.BuildSelector<T, T>(graph));
            var result = new RoynResult();
            result.SetResult(ds.ToList(), roynRequest.Columns.ToArray());

            return result;
        }

        //public static RoynResult RoynSelect<T>(this IQueryable<T> source, RequestGraph graph) where T : class
        //{
        //    var s = LambdaBuilder.BuildSelector<T, T>(graph);
        //    var dataSource2 = source.Select(s);

        //    var result = new RoynResult();
        //    result.SetResult(dataSource2.ToList(), graph);
        //    return result;
        //}

        public static RoynResult RoynSelect<T, TResult>(this DbSet<T> source, RoynRequest<T> roynRequest)
            where T : class
            where TResult : class
        {
            var includeSource = HandleIncludes(source.AsNoTracking(), roynRequest.Columns.ToArray());
            var filteredSource = HandleFilter(includeSource, roynRequest);
            var orderedDataSource = HandleOrder(filteredSource, roynRequest);
            var dataSource = HandleSkipTake(orderedDataSource, roynRequest);
            //  var resultsSource = dataSource.Select(LambdaBuilder.BuildSelector<T, TResult>(roynRequest.Columns.ToArray()));

            var result = new RoynResult();
            result.SetResult(dataSource.ToList(), roynRequest.Columns.ToArray());
            return result;
        }

        private static IQueryable<T> HandleSkipTake<T>(IOrderedQueryable<T> querable, RoynRequest<T> roynRequest) where T : class
        {
            if (roynRequest.InternalSkipSize == null || roynRequest.InternalTakeSize == null)
            {
                return querable;
            }

            return querable.Skip(roynRequest.InternalSkipSize.Value).Take(roynRequest.InternalTakeSize.Value);
        }

        private static IQueryable<T> HandleIncludes<T>(IQueryable<T> source, string[] columns) where T : class
        {
            var newSource = source;
            var includes = columns.Where(x => x.Contains(".")).Select(x => Path.GetFileNameWithoutExtension(x)).Distinct().ToList();
            foreach (var i in includes)
            {
                newSource = newSource.Include(i);
            }
            return newSource;
        }

        private static DbQuery<T> HandleIncludes<T>(DbQuery<T> source, string[] columns) where T : class
        {
            DbQuery<T> q = source as DbQuery<T>;
            var includes = columns.Where(x => x.Contains(".")).Select(x => Path.GetFileNameWithoutExtension(x)).Distinct().ToList();
            foreach (var i in includes)
            {
                q = q.Include(i);
            }
            return q;
        }

        //public static IQueryable<TResult> Select<TResult>(this IQueryable source, string[] columns)
        //{
        //    var sourceType = source.ElementType;
        //    var resultType = typeof(TResult);
        //    var parameter = Expression.Parameter(sourceType, "e");
        //    var bindings = columns.Select(column => Expression.Bind(
        //        resultType.GetProperty(column), Expression.PropertyOrField(parameter, column)));
        //    var body = Expression.MemberInit(Expression.New(resultType), bindings);
        //    var selector = Expression.Lambda(body, parameter);
        //    return source.Provider.CreateQuery<TResult>(
        //        Expression.Call(typeof(Queryable), "Select", new Type[] { sourceType, resultType },
        //            source.Expression, Expression.Quote(selector)));
        //}

        //public static Func<T, TResult> CreateNewStatement<T, TResult>(string[] columns)
        //{
        //    // input parameter "o"
        //    var xParameter = Expression.Parameter(typeof(T), "o");

        //    // new statement "new Data()"
        //    var xNew = Expression.New(typeof(TResult));

        //    // create initializers
        //    var bindings = columns
        //        .Select(o =>
        //        {
        //            // property "Field1"
        //            var mi = typeof(T).GetProperty(o);

        //            // original value "o.Field1"
        //            var xOriginal = Expression.Property(xParameter, mi);

        //            // set value "Field1 = o.Field1"
        //            return Expression.Bind(mi, xOriginal);
        //        }
        //    );

        //    // initialization "new Data { Field1 = o.Field1, Field2 = o.Field2 }"
        //    var xInit = Expression.MemberInit(xNew, bindings);

        //    // expression "o => new Data { Field1 = o.Field1, Field2 = o.Field2 }"
        //    var lambda = Expression.Lambda<Func<T, TResult>>(xInit, xParameter);

        //    return lambda.Compile();
        //}
    }
}