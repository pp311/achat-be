using System.Linq.Expressions;
using AChat.Application.Common.Models;
using AChat.Domain.Entities.Base;

namespace AChat.Application.Common.Extensions;

public static class QueryableExtension
{
    public static IQueryable<TEntity> WhereIf<TEntity>(
        this IQueryable<TEntity> query, 
        bool condition, 
        Expression<Func<TEntity, bool>> predicate) where TEntity : EntityBase 
        => condition ? query.Where(predicate) : query;
    
    public static IQueryable<TEntity> OrderBy<TEntity>(
        this IQueryable<TEntity> query, 
        IOrderByField orderByField,
        bool isDescending) where TEntity : EntityBase
        => isDescending 
            ? Queryable.OrderByDescending(query, orderByField.GetExpression()) 
            : Queryable.OrderBy(query, orderByField.GetExpression());
    
    public static IQueryable<TEntity> OrderBy<TEntity, TField>(
        this IQueryable<TEntity> query, 
        Expression<Func<TEntity, TField>> fieldSelector,
        bool isDescending) where TEntity : class
        => isDescending 
            ? query.OrderByDescending(fieldSelector) 
            : query.OrderBy(fieldSelector);
}