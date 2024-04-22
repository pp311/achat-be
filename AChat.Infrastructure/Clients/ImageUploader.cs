using AChat.Application.Common.Dtos;
using AChat.Application.Common.Interfaces;
using AChat.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace AChat.Infrastructure.Clients;

public class ImageUploader(ILogger<ImageUploader> logger) : IImageUploader
{
    public async Task<MediaUrlDto> UploadAvatarImageAsync(string fileFileName, Stream openReadStream)
    {
        var httpClient = new HttpClient();
        
        var content = new MultipartFormDataContent();
        content.Add(new StreamContent(openReadStream), "media", fileFileName);
        var response = await httpClient.PostAsync("https://thumbor.whitemage.tech/image", content);
        
        if (response.IsSuccessStatusCode)
        {
            var relativePath = response.Headers.GetValues("Location").FirstOrDefault();
            var url = $"https://thumbor.whitemage.tech{relativePath}";
            return new MediaUrlDto { Url = url };
        }

        var result = await response.Content.ReadAsStringAsync();
        logger.LogError("Upload file {name} failed: {error}", fileFileName, result);
        throw new AppException("Cannot upload file");
    }

    public Task<MediaUrlDto> UploadPropertyImageAsync(string fileName, Stream stream)
    {
        throw new NotImplementedException();
    }

    public Task<MediaUrlDto> UploadMediaFileAsync(string fileName, Stream stream)
    {
        throw new NotImplementedException();
    }
}