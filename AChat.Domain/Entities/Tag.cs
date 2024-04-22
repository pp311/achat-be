using AChat.Domain.Entities.Base;

namespace AChat.Domain.Entities;

public class Tag : AuditableEntity
{
    public string Name { get; set; } = null!; 
    public int UserId { get; set; }
    public string Color { get; set; } = null!;
    
    public ICollection<Contact> Contacts { get; set; } = new HashSet<Contact>();
}