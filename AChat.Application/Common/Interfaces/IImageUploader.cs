using AChat.Application.Common.Dtos;

namespace AChat.Application.Common.Interfaces;

public interface IImageUploader
{
    public Task<MediaUrlDto> UploadAvatarImageAsync(string fileName, Stream stream);
    public Task<MediaUrlDto> UploadPropertyImageAsync(string fileName, Stream stream);
    public Task<MediaUrlDto> UploadMediaFileAsync(string fileName, Stream stream);
}