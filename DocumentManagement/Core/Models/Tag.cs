﻿namespace DocumentManagement.Core.Models;

public class Tag
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ICollection<DocumentTag> DocumentTags { get; set; }
}