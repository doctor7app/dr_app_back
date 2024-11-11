namespace Common.Extension;

public static class TypeExtensions
{
    public static bool ImplementsGenericInterface(this Type type, Type interfaceType)
    {
        return type.GetInterfaces().Any(i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);
    }
}
