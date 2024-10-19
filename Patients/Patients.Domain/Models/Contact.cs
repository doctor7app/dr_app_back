﻿using Doctor.Common.Classes;
using Doctor.Common.Enums;

namespace Patients.Domain.Models;

public class Contact : AuditableEntity
{
    public Guid ContactId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public ContactType Type { get; set; }

    public Guid FkIdPatient { get; set; }
    public Patient Patient { get; set; }
}