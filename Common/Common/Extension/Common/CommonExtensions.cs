namespace Common.Extension.Common;

public static class CommonExtensions
{
    public static bool IsNullOrEmptyGuid(this Guid? guid)
    {
        return !guid.HasValue || guid.Value == Guid.Empty;
    }


    public static bool IsNullOrEmptyGuid(this Guid guid)
    {
        return guid == Guid.Empty;
    }
}