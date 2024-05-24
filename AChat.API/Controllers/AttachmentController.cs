using AChat.Application.Common.Configurations;
using AChat.Application.Common.Interfaces;
using AChat.Application.ViewModels.Message;
using AChat.Domain;
using AChat.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;

namespace AChat.Controllers;

[ApiController]
[Route("api/attachments")]
public class AttachmentController : ControllerBase
{
    private readonly IImageUploader _imageUploader;
    private readonly IMinioClient _minioClient;
    private readonly MinioSettings _minioSettings;

    public AttachmentController(IImageUploader imageUploader, IMinioClient minioClient, IOptions<MinioSettings> minioSettings)
    {
        _imageUploader = imageUploader;
        _minioClient = minioClient;
        _minioSettings = minioSettings.Value;
    }

    [HttpPost("avatar")]
    public async Task<IActionResult> UploadAvatarImage([FromForm] IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is empty");

        ValidateImage(file);

        var url = await _imageUploader.UploadAvatarImageAsync(file.FileName, file.OpenReadStream());
        return Ok(url);
    }

    [HttpPost("property")]
    public async Task<IActionResult> UploadPropertyImage([FromForm] IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is empty");

        ValidateImage(file);

        var url = await _imageUploader.UploadAvatarImageAsync(file.FileName, file.OpenReadStream());
        return Ok(url);
    }

    [HttpPost("media")]
    public async Task<IActionResult> UploadMediaFile([FromForm] IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is empty");

        ValidateMedia(file);

        var url = await _imageUploader.UploadAvatarImageAsync(file.FileName, file.OpenReadStream());
        return Ok(url);
    }
    
    [HttpPost("upload-facebook-attachment")]
    public async Task<IActionResult> TestMinio([FromForm] IFormFile file)
    {
        var fileType = ValidateFacebookAttachment(Path.GetExtension(file.FileName), file.Length);
        
        if (!await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_minioSettings.BucketName)))
            return BadRequest("Bucket does not exist");
        using var newMemoryStream = new MemoryStream();
        
        var fileName = file.FileName;
        
        await file.CopyToAsync(newMemoryStream);
        
        var contentType = file.ContentType;
        
        var size = newMemoryStream.Length;
        newMemoryStream.Position = 0;
        var args = new PutObjectArgs()
            .WithBucket(_minioSettings.BucketName)
            .WithObject($"{fileName}")
            .WithObjectSize(size)
            .WithContentType(contentType)
            .WithStreamData(newMemoryStream);
        
        var response = await _minioClient.PutObjectAsync(args);

        return Ok(new UploadFacebookAttachmentResponse
        {
            Type = fileType.ToValue(),
            Url = $"{_minioSettings.BaseUrl}/{fileName}"
        });
    }
    
    [HttpPost("upload-gmail-attachment")]
    public async Task<IActionResult> UploadGmailAttachment([FromForm] List<IFormFile>? files)
    {
        if (files == null || files.Count == 0)
            return BadRequest("File is empty");

        var urls = new List<string>();
        var tasks = new List<Task>();
        foreach (var file in files)
        {
            var fileType = ValidateFacebookAttachment(Path.GetExtension(file.FileName), file.Length);
            
            if (!await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_minioSettings.BucketName)))
                return BadRequest("Bucket does not exist");
            using var newMemoryStream = new MemoryStream();
            
            var fileName = file.FileName;
            
            await file.CopyToAsync(newMemoryStream);
            
            var contentType = file.ContentType;
            
            var size = newMemoryStream.Length;
            newMemoryStream.Position = 0;
            var args = new PutObjectArgs()
                .WithBucket(_minioSettings.BucketName)
                .WithObject($"{fileName}")
                .WithObjectSize(size)
                .WithContentType(contentType)
                .WithStreamData(newMemoryStream);
            
            tasks.Add(_minioClient.PutObjectAsync(args));
            urls.Add($"{_minioSettings.BaseUrl}/{fileName}");
        }
        
        await Task.WhenAll(tasks);

        return Ok(urls);
    }

    private static FacebookAttachmentType ValidateFacebookAttachment(string fileExtension, long fileSize)
    {
        var fileType = FacebookConstant.GetAttachmentType(fileExtension);
        
        // validate file size
        // image: 5MB
        // audio & video & file: 16MB
        
        if (fileType == FacebookAttachmentType.Image && fileSize > 1024 * 1024 * 5)
            throw new AppException("Image file size limit exceeded");
        
        if ((fileType == FacebookAttachmentType.Audio || fileType == FacebookAttachmentType.Video || fileType == FacebookAttachmentType.File) && fileSize > 1024 * 1024 * 16)
            throw new AppException("Attachment size limit exceeded");

        return fileType;
    }

    private static void ValidateImage(IFormFile file)
    {
        var ext = Path.GetExtension(file.FileName).ToLower();
        if (ext != ".jpg" && ext != ".png" && ext != ".jpeg")
            throw new AppException("Invalid media format");

        if (file.ContentType != "image/jpeg" && file.ContentType != "image/png")
            throw new AppException("Invalid media format");

        if (file.Length > 1024 * 1024 * 5)
            throw new AppException("Media file size limit exceeded");
    }

    // for both image and video
    private static void ValidateMedia(IFormFile file)
    {
        var ext = Path.GetExtension(file.FileName).ToLower();
        if (ext != ".jpg" && ext != ".png" && ext != ".jpeg" && ext != ".mp4")
            throw new AppException("Invalid media format");

        if (file.ContentType != "image/jpeg" && file.ContentType != "image/png" && file.ContentType != "video/mp4")
            throw new AppException("Invalid media format");

        if (file.Length > 1024 * 1024 * 10)
            throw new AppException("Media file size limit exceeded");
    }
}