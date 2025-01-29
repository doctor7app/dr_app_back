using Common.Enums;

namespace Contracts.Messages.Patients;

public class ContactCreatedEvent
{
    public Guid ContactId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public ContactType Type { get; set; }
}