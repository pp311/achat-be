using AChat.Application.Common.Interfaces;
using AChat.Application.ViewModels.Source.Responses;
using AChat.Domain;
using AChat.Domain.Entities;
using AChat.Domain.Exceptions;
using AChat.Domain.Repositories.Base;
using AutoMapper;

namespace AChat.Application.Services;

public class SourceService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUser currentUser,
    IFacebookClient facebookClient,
    IRepositoryBase<Source> sourceRepository) : BaseService(unitOfWork, mapper, currentUser)
{
    public async Task ConnectFacebookAsync(string accessToken)
    {
        var facebookInfo = await facebookClient.GetPageInfoAsync(accessToken)
            ?? throw new Exception("Failed to get Facebook page info");
        
        var userPages = await facebookClient.GetPageLongLiveTokenAsync(accessToken, facebookInfo.Id)
            ?? throw new Exception("Failed to get user pages");
        
        var existingSources = await sourceRepository.GetListAsync(_ => _.UserId == CurrentUser.Id);

        try
        {
            foreach (var page in userPages.Data)
            {
                if (existingSources.Any(_ => _.PageId == page.Id))
                    continue;
                
                await facebookClient.SubscribeAppAsync(page.AccessToken, page.Id);
                
                var source = new Source
                {
                    Type = SourceType.Facebook,
                    AccessToken = page.AccessToken,
                    PageId = page.Id,
                    Name = page.Name,
                    UserId = CurrentUser.Id
                };
               
                sourceRepository.Add(source);
            }
        }
        catch
        {
            await facebookClient.UnsubscribeAppsAsync(accessToken, userPages.Data.Select(_ => _.Id).ToList());
            throw new AppException("Failed to connect Facebook page"); 
        } 
        await UnitOfWork.SaveChangesAsync();
    } 
    
    public async Task DisconnectFacebookAsync(string pageId)
    {
        var source = await sourceRepository.GetAsync(_ => _.PageId == pageId && _.UserId == CurrentUser.Id)
            ?? throw new NotFoundException(nameof(Source), pageId);
        
        await facebookClient.UnsubscribeAppsAsync(source.AccessToken, new List<string> { pageId });
        
        sourceRepository.Delete(source);
        await UnitOfWork.SaveChangesAsync();
    }
    
    public async Task<List<SourceResponse>> GetSourcesAsync()
    {
        var sources = await sourceRepository.GetListAsync(_ => _.UserId == CurrentUser.Id);
        return Mapper.Map<List<SourceResponse>>(sources);
    }
}
