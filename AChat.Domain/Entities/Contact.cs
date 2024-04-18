using AChat.Domain.Entities.Base;

namespace AChat.Domain.Entities;

public class Contact : AuditableEntity
{
    public string? Name { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    public Gender Gender { get; set; } = Gender.Unknown;

    public string? AvatarUrl { get; set; }

    public bool IsHidden { get; set; }

    public int UserId { get; set; }

    public string? FacebookUserId { get; set; }

    public string? CustomInfo { get; set; }

    public int? SourceId { get; set; }

    public Source? Source { get; set; }
    public ICollection<Note> Notes { get; set; } = new HashSet<Note>();
    public ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
    public ICollection<Message> Messages { get; set; } = new HashSet<Message>();
}