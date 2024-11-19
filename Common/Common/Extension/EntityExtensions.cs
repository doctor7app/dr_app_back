using System.Reflection;

namespace Common.Extension;

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
}