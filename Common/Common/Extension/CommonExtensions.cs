namespace Common.Extension;

public static class CommonExtensions
{
    public static bool IsNullOrEmpty(this Guid? guid)
    {
        return !guid.HasValue || guid.Value == Guid.Empty;
    }


    public static bool IsNullOrEmpty(this Guid guid)
    {
        return guid == Guid.Empty;
    }
}