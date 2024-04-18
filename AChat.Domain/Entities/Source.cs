using AChat.Domain.Entities.Base;

namespace AChat.Domain.Entities;

public class Source : AuditableEntity
{
    public SourceType Type { get; set; }  
    
    public string? AccessToken { get; set; } 
    public string? RefreshToken { get; set; } 
    public string? PageId { get; set; }
    public string? PageName { get; set; }
    public int UserId { get; set; }
}