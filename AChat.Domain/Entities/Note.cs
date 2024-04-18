using AChat.Domain.Entities.Base;

namespace AChat.Domain.Entities;

public class Note : AuditableEntity
{
    public string Content { get; set; } = null!;
    public int UserId { get; set; } 
    public int ContactId { get; set; }
    
    public Contact Contact { get; set; } = null!;
}