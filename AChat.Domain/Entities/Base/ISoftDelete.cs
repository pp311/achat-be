namespace AChat.Domain.Entities.Base;

public interface ISoftDelete
{
	public bool IsDeleted { get; set; }
}