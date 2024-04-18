using AChat.Application.Common.Interfaces;
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

        try
        {
            foreach (var page in userPages.Data)
            {
                await facebookClient.SubscribeAppAsync(page.AccessToken, page.Id);
                
                var source = new Source
                {
                    Type = SourceType.Facebook,
                    AccessToken = page.AccessToken,
                    PageId = page.Id,
                    PageName = page.Name,
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
}
