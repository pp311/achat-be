using AChat.Domain.Entities;
using AChat.Domain.Repositories.Base;

namespace AChat.Domain.Repositories;

public interface IContactRepository : IRepositoryBase<Contact>
{
    public IQueryable<Contact> Search(string? search); 
}