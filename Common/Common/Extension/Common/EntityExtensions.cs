using System.Reflection;

namespace Common.Extension.Common;

public static class EntityExtensions
{
    public static void UpdateWithDto<TEntity, TDto>(this TEntity entity, TDto dto)
        where TEntity : class
        where TDto : class
    {
        if (entity == null || dto == null)
            throw new ArgumentNullException($"Entity or DTO cannot be null.");

        var dtoProperties = typeof(TDto).GetProperties();
        foreach (var dtoProperty in dtoProperties)
        {
            var dtoValue = dtoProperty.GetValue(dto);

            if (dtoValue == null || IsDefaultValue(dtoValue))
                continue;

            var entityProperty = typeof(TEntity).GetProperty(dtoProperty.Name);
            if (entityProperty != null && entityProperty.CanWrite)
            {
                if (IsCollection(entityProperty.PropertyType))
                {
                    UpdateCollection(entityProperty, entity, dtoValue);
                }
                else
                {
                    entityProperty.SetValue(entity, dtoValue);
                }
            }
        }
    }

    private static bool IsDefaultValue(object value)
    {
        var type = value.GetType();
        return type.IsValueType && value.Equals(Activator.CreateInstance(type));
    }

    private static bool IsCollection(Type type)
    {
        return type != typeof(string) && typeof(System.Collections.IEnumerable).IsAssignableFrom(type);
    }

    private static void UpdateCollection(PropertyInfo entityProperty, object entity, object dtoValue)
    {
        var entityCollection = entityProperty.GetValue(entity) as System.Collections.IList;
        var dtoCollection = dtoValue as System.Collections.IEnumerable;

        if (entityCollection == null || dtoCollection == null)
            return;

        entityCollection.Clear();
        foreach (var item in dtoCollection)
        {
            entityCollection.Add(item);
        }
    }

    /// <summary>
    /// Compares two objects of the same type for equality based on their public properties.
    /// </summary>
    /// <param name="entity">The first object to compare.</param>
    /// <param name="other">The second object to compare.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public static bool AreEqual<T>(this T entity, T other) where T : class
    {
        if (entity == null || other == null || entity.GetType() != other.GetType())
            return false;

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead);

        foreach (var property in properties)
        {
            var value1 = property.GetValue(entity);
            var value2 = property.GetValue(other);

            if (!Equals(value1, value2))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Generates a hash code for the object based on its public properties.
    /// </summary>
    /// <param name="entity">The object to generate a hash code for.</param>
    /// <returns>A hash code representing the object.</returns>
    public static int GetHashCode<T>(this T entity) where T : class
    {
        if (entity == null) return 0;

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead);

        int hash = 17;
        foreach (var property in properties)
        {
            var value = property.GetValue(entity);
            hash = hash * 23 + (value?.GetHashCode() ?? 0);
        }

        return hash;
    }
}