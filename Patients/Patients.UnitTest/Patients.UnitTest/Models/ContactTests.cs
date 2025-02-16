using Common.Enums;
using Common.Extension.Common;
using Patients.Domain.Models;

namespace Patients.UnitTest.Models;

public class ContactTests
{
    [Fact]
    public void Contact_Constructor_SetsProperties()
    {
        // Arrange
        var contact = new Contact
        {
            ContactId = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "1234567890",
            Email = "john.doe@example.com",
            FkIdPatient = Guid.NewGuid(),
            Type = ContactType.Work
        };

        // Act

        // Assert
        Assert.NotNull(contact);
        Assert.Equal("John", contact.FirstName);
        Assert.Equal("Doe", contact.LastName);
        Assert.Equal("1234567890", contact.PhoneNumber);
        Assert.Equal("john.doe@example.com", contact.Email);
        Assert.Equal(1, (double)contact.Type);
    }

    [Fact]
    public void Contact_Equality_ChecksEqualObjects()
    {
        // Arrange
        var contact1 = new Contact { ContactId = Guid.NewGuid(), FirstName = "John" };
        var contact2 = new Contact { ContactId = contact1.ContactId, FirstName = "John" };

        // Act

        // Assert
        Assert.True(contact1.AreEqual(contact2));
    }
}