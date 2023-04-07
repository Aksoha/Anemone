using System.Linq.Expressions;
using System.Reflection;

namespace Anemone.RepositoryMock;

internal static class PropertyRetriever
{
    public static Action<TClass, TProperty> GetSetterForProperty<TClass, TProperty>(Expression<Func<TClass, TProperty>> selector)where TClass : class
    {
        var expression = selector.Body;
        var propertyInfo = GetPropertyInfo(expression);
        
        return GetPropertySetter<TClass, TProperty>(propertyInfo);
    }

    private static PropertyInfo GetPropertyInfo(Expression expression)
    {
        var propertyInfo = expression.NodeType == ExpressionType.MemberAccess
            ? (PropertyInfo)((MemberExpression)expression).Member
            : null;
        
        if (propertyInfo is null)
            throw new InvalidOperationException("The expression body must be a field or property");
        
        return propertyInfo;
    }

    private static Action<TObj, TProperty> GetPropertySetter<TObj, TProperty>(PropertyInfo propertyInfo)
    {
        var setter = propertyInfo.GetSetMethod(nonPublic: true);
        if (setter is null)
            throw new InvalidOperationException(
                $"The property {propertyInfo.DeclaringType?.FullName}.{propertyInfo.Name} does not have a setter");

        return (obj, value) => setter.Invoke(obj, new object?[] { value });
    }
}