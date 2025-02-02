namespace Notify.Core.Options;

public class EmailSettings
{
    public string SmtpHost { get; set; }
    public int SmtpPort { get; set; }
    public string SmtpUser { get; set; }
    public string SmtpPass { get; set; }
    public string SmtpFrom { get; set; }
    public string SmtpName { get; set; }
}
