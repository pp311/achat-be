namespace AChat.Domain.Entities.Base;

public interface IAuditableEntity : IEntityBase
{
    int? CreatedBy { get; set; }  
    DateTime? CreatedOn { get; set; }
    int? UpdatedBy { get; set; }
    DateTime? UpdatedOn {get;set;}
}