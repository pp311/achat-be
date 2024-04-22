using AChat.Application.Common.Interfaces;
using AChat.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace AChat.Controllers;

[ApiController]
[Route("api/attachments")]
public class AttachmentController : ControllerBase
{
    private readonly IImageUploader _imageUploader;

    public AttachmentController(IImageUploader imageUploader)
    {
        _imageUploader = imageUploader;
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