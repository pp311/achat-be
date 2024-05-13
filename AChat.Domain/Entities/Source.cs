using AChat.Domain.Entities.Base;

namespace AChat.Domain.Entities;

public class Source : AuditableEntity, ISoftDelete
{
    public SourceType Type { get; set; }

    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? PageId { get; set; }
    public string? Name { get; set; }
    public string? Email {get; set;}
    public int UserId { get; set; }
    public bool IsDeleted { get; set; }
    public ulong HistoryId { get; set; }
}