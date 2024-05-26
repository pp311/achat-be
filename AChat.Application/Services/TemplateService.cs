using AChat.Application.Common.Extensions;
using AChat.Application.Common.Interfaces;
using AChat.Application.ViewModels.Template;
using AChat.Domain;
using AChat.Domain.Entities;
using AChat.Domain.Exceptions;
using AChat.Domain.Repositories.Base;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace AChat.Application.Services;

public class TemplateService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUser currentUser,
    IRepositoryBase<Template> repository) : BaseService(unitOfWork, mapper, currentUser)
{
    public async Task<List<TemplateResponse>> GetTemplatesAsync(CancellationToken ct)
    {
        return await repository
            .GetQuery(_ => _.UserId == CurrentUser.Id)
            .OrderByDescending(_ => _.CreatedOn)
            .ProjectTo<TemplateResponse>(Mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }
    
    public async Task<TemplateResponse> GetTemplateAsync(int id, CancellationToken ct)
    {
        return await repository.GetQuery()
            .Where(_ => _.Id == id && _.UserId == CurrentUser.Id)
            .ProjectTo<TemplateResponse>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct) ?? throw new NotFoundException(nameof(Template), id.ToString());
    }
    
    public async Task<List<TemplateLookupResponse>> GetTemplateLookupAsync(TemplateType? type, CancellationToken ct)
    {
        return await repository.GetQuery()
            .Where(_ => _.UserId == CurrentUser.Id)
            .WhereIf(type != null, _ => _.Type == type) 
            .OrderBy(_ => _.Name)
            .ProjectTo<TemplateLookupResponse>(Mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }
    
    public async Task<int> CreateTemplateAsync(CreateTemplateRequest request, CancellationToken ct)
    {
        var template = new Template
        {
            Name = request.Name,
            Content = request.Content,
            Type = request.Type,
            UserId = CurrentUser.Id
        };
        repository.Add(template);
        await UnitOfWork.SaveChangesAsync(ct);
        return template.Id;
    }
    
    public async Task UpdateTemplateAsync(int id, UpdateTemplateRequest request, CancellationToken ct)
    {
        var template = await repository.GetAsync(_ => _.Id == id, ct) 
                       ?? throw new NotFoundException(nameof(Template), id.ToString());
        
        template.Name = request.Name;
        template.Content = request.Content;
        await UnitOfWork.SaveChangesAsync(ct);
    }
    
    public async Task DeleteTemplateAsync(int id, CancellationToken ct)
    {
        var template = await repository.GetAsync(_ => _.Id == id, ct) 
                       ?? throw new NotFoundException(nameof(Template), id.ToString());
        
        repository.Delete(template);
        await UnitOfWork.SaveChangesAsync(ct);
    }
}