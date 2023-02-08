using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using AzureTestAbstract.Implementation;

namespace AzureTestAbstract.Helpers;

public class MyExpressionVisitor : ExpressionVisitor
{
    private ReadOnlyCollection<ParameterExpression> _parameters;

    public static Expression<Func<TableItem, bool>> Convert<T>(Expression<T> root)
    {
        var visitor = new MyExpressionVisitor();
        var expression = (Expression<Func<TableItem, bool>>)visitor.Visit(root);
        return expression;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return _parameters != null ? _parameters.FirstOrDefault(p => p.Name == node.Name) :
            node.Type == typeof(IAbstractTableItem) ? Expression.Parameter(typeof(TableItem), node.Name) : node;
    }

    protected override Expression VisitLambda<T>(Expression<T> node)
    {
        _parameters = VisitAndConvert(node.Parameters, "VisitLambda");
        return Expression.Lambda(Visit(node.Body), _parameters);
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Member.DeclaringType == typeof(IAbstractTableItem))
            return Expression.MakeMemberAccess(Visit(node.Expression), typeof(TableItem).GetProperty(node.Member.Name));
        return base.VisitMember(node);
    }
}
class Visitor<T> : ExpressionVisitor
{
    ParameterExpression _parameter;

    public Visitor(ParameterExpression parameter)
    {
        _parameter = parameter;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return _parameter;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Member.MemberType != System.Reflection.MemberTypes.Property)
            return base.VisitMember(node);

        var memberName = node.Member.Name;
        var otherMember = typeof(T).GetProperty(memberName);
        var inner = Visit(node.Expression);
        return Expression.Property(inner, otherMember);
    }
}
