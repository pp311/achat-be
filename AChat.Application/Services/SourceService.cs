using AChat.Application.Common.Interfaces;
using AChat.Application.ViewModels.Source.Responses;
using AChat.Domain;
using AChat.Domain.Entities;
using AChat.Domain.Exceptions;
using AChat.Domain.Repositories.Base;
using AutoMapper;
using Message = AChat.Domain.Entities.Message;

namespace AChat.Application.Services;

public class SourceService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUser currentUser,
    IRepositoryBase<Message> messageRepository,
    IRepositoryBase<Contact> contactRepository,
    IFacebookClient facebookClient,
    IGmailClient gmailClient,
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
                var existingSource = existingSources.Find(_ => _.PageId == page.Id);
                if (existingSource != null)
                {
                    if (!existingSource.IsDeleted)
                        continue;

                    existingSource.AccessToken = page.AccessToken;
                    existingSource.IsDeleted = false;
                    sourceRepository.Update(existingSource);
                }
                else
                {
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

                await facebookClient.SubscribeAppAsync(page.AccessToken, page.Id);
            }
        }
        catch
        {
            await facebookClient.UnsubscribeAppsAsync(accessToken, userPages.Data.Select(_ => _.Id).ToList());
            throw new AppException("Failed to connect Facebook page");
        }
        await UnitOfWork.SaveChangesAsync();
    }

    public async Task DisconnectFacebookAsync(int sourceId)
    {
        var source = await sourceRepository.GetAsync(_ => _.Id == sourceId && _.UserId == CurrentUser.Id)
            ?? throw new NotFoundException(nameof(Source), sourceId.ToString());

        var isOtherSourceUseThisEmail = await sourceRepository.AnyAsync(_ => _.PageId == source.PageId
                                                                             && _.UserId != CurrentUser.Id
                                                                             && _.Type == SourceType.Facebook
                                                                             && !_.IsDeleted);

        if (!isOtherSourceUseThisEmail)
        {
            await facebookClient.UnsubscribeAppsAsync(source.AccessToken, [source.PageId!]);
        }

        sourceRepository.Delete(source);
        await UnitOfWork.SaveChangesAsync();
    }

    public async Task<List<SourceResponse>> GetSourcesAsync()
    {
        var sources = await sourceRepository.GetListAsync(_ => _.UserId == CurrentUser.Id && !_.IsDeleted);
        return Mapper.Map<List<SourceResponse>>(sources);
    }

    public async Task ConnectGmailAsync(string code)
    {
        var (accessToken, refreshToken) = await gmailClient.GetCredentialFromCodeAsync(code);

        var credential = gmailClient.GetUserCredentialAsync(accessToken, refreshToken);

        var gmailInfo = await gmailClient.GetInfoAsync(credential)
            ?? throw new AppException("Failed to get Gmail info");

        var existingSource = await sourceRepository.GetAsync(_ => _.UserId == CurrentUser.Id && _.Email == gmailInfo.Email);

        var historyId = await gmailClient.GetHistoryIdAsync(credential);

        if (existingSource != null)
        {
            existingSource.Name = gmailInfo.Name;
            existingSource.AccessToken = accessToken;
            existingSource.RefreshToken = refreshToken;
            existingSource.HistoryId = historyId;
            existingSource.IsDeleted = false;
            sourceRepository.Update(existingSource);
        }
        else
        {
            var source = new Source
            {
                Type = SourceType.Gmail,
                Name = gmailInfo.Name,
                Email = gmailInfo.Email,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                HistoryId = historyId,
                UserId = CurrentUser.Id
            };

            sourceRepository.Add(source);
        }

        var isOtherSourceUseThisEmail = await sourceRepository.AnyAsync(_ => _.Email == gmailInfo.Email
                                                                             && _.UserId != CurrentUser.Id
                                                                             && _.Type == SourceType.Gmail
                                                                             && !_.IsDeleted);

        if (!isOtherSourceUseThisEmail)
            await gmailClient.SubscribeAsync(credential);


        await UnitOfWork.SaveChangesAsync();
    }

    public async Task DisconnectGmailAsync(int sourceId)
    {
        var source = await sourceRepository
            .GetAsync(_ => _.Id == sourceId && _.UserId == CurrentUser.Id && _.Type == SourceType.Gmail)
            ?? throw new NotFoundException(nameof(Source), sourceId.ToString());

        var isOtherSourceUseThisEmail = await sourceRepository.AnyAsync(_ => _.Email == source.Email
                                                                             && _.UserId != CurrentUser.Id
                                                                             && _.Type == SourceType.Gmail
                                                                             && !_.IsDeleted);

        if (!isOtherSourceUseThisEmail)
        {
            var credential = gmailClient.GetUserCredentialAsync(source.AccessToken!, source.RefreshToken!);
            await gmailClient.UnsubscribeAsync(credential);
        }

        sourceRepository.Delete(source);

        contactRepository.DeleteRange(await contactRepository.GetListAsync(_ => _.SourceId == sourceId));

        await UnitOfWork.SaveChangesAsync();
    }
}
