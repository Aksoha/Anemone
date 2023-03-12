using System.Linq.Expressions;
using System.Reflection;

namespace Anemone.RepositoryMock;

internal static class PropertyHelper
{
    public static Action<T, TValue>? GetSetterForProperty<T, TValue>(Expression<Func<T, TValue>> selector)where T : class
    {
        var expression = selector.Body;
        var propertyInfo = expression.NodeType == ExpressionType.MemberAccess
            ? (PropertyInfo)((MemberExpression)expression).Member
            : null;
        return propertyInfo is null ? null : GetPropertySetter<T, TValue>(propertyInfo);
    }

    private static Action<T, TValue> GetPropertySetter<T, TValue>(PropertyInfo propertyInfo)
    {
        var setter = propertyInfo.GetSetMethod(nonPublic: true);
        if (setter is null)
            throw new InvalidOperationException(
                $"The property {propertyInfo.DeclaringType?.FullName}.{propertyInfo.Name} does not have a setter");

        return (obj, value) => setter.Invoke(obj, new object?[] { value });
    }
}