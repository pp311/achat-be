using AChat.Domain.Entities.Base;

namespace AChat.Domain.Entities;

public class Template : AuditableEntity
{
    public string Name { get; set; } = null!;
    public string Content { get; set; } = null!;
    public int UserId { get; set; }
    public TemplateType Type { get; set; }
}
