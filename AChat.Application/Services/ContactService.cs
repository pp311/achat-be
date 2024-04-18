using AChat.Application.Common.Interfaces;
using AChat.Domain.Repositories.Base;
using AutoMapper;

namespace AChat.Application.Services;

public class ContactService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUser currentUser) : BaseService(unitOfWork, mapper, currentUser)
{
    
}