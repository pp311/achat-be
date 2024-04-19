using AChat.Domain.Entities;
using AChat.Domain.Repositories;
using AChat.Infrastructure.Data;
using AChat.Infrastructure.Repositories.Base;

namespace AChat.Infrastructure.Repositories;

public class ContactRepository : RepositoryBase<Contact>, IContactRepository
{
    public ContactRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public IQueryable<Contact> Search(string? search)
        => string.IsNullOrWhiteSpace(search) 
            ? GetQuery() 
            : GetQuery(b => (!string.IsNullOrEmpty(b.Name) && b.Name.Contains(search)));
}