using AutoMapper;

namespace Common.Helpers.Converters;

public class UtcDateTimeConverter : IValueConverter<DateTime?, DateTime?>
{
    public DateTime? Convert(DateTime? source, ResolutionContext context)
    {
        if (!source.HasValue)
            return null;

        return source.Value.Kind switch
        {
            DateTimeKind.Utc => source.Value,
            DateTimeKind.Local => source.Value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(source.Value, DateTimeKind.Utc)
        };
    }
}