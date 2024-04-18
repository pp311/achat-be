using System.Linq.Expressions;
using AChat.Domain.Entities.Base;

namespace AChat.Application.Common.Models;

public interface IOrderByField
{
	dynamic GetExpression();
}

public class OrderByField<TEntity, TField> : IOrderByField where TEntity : EntityBase
{
	private readonly Expression<Func<TEntity, TField>> _expression;
    	
    public OrderByField(Expression<Func<TEntity, TField>> expression)
    {
    	_expression = expression;
    } 
    
    public dynamic GetExpression() => _expression;
}
