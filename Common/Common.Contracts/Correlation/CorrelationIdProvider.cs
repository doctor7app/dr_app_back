namespace Common.Contracts.Correlation;

public static class CorrelationIdProvider
{
    public static Guid GenerateCorrelationId()
    {
        return Guid.NewGuid();
    }

    public static Guid? GetCorrelationId(IDictionary<string, object> headers)
    {
        if (headers != null && headers.TryGetValue("CorrelationId", out var correlationId))
        {
            if (Guid.TryParse(correlationId?.ToString(), out var parsedId))
            {
                return parsedId;
            }
        }
        return null;
    }
}
