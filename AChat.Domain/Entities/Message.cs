using AChat.Domain.Entities.Base;

namespace AChat.Domain.Entities;

public class Message : AuditableEntity
{
    public int ContactId { get; set; }  
    public string Content { get; set; } = null!;
    public string? Subject { get; set; }
    public string? MId { get; set; }
    
    public bool IsEcho { get; set; }
    
    public Contact Contact { get; set; } = null!;
    
    public ICollection<MessageAttachment> Attachments { get; set; } = new HashSet<MessageAttachment>();
}

public class MessageAttachment
{
    public string Url { get; set; } = null!;
    public string? FileName { get; set; }
    public string? Type { get; set; }
}