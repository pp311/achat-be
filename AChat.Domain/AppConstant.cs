using AChat.Domain.Exceptions;

namespace AChat.Domain;

public static class AppConstant
{
    public const int DefaultPageNumber = 1; 
    public const int DefaultPageSize = 10;
}

public static class StringLength
{
    public const int Name = 256;
    public const int Description = 2000;
    public const int Email = 256;
    public const int Phone = 32;
    public const int Url = 512;
    public const int Token = 512;
    public const int ConfigurationJson = 4000;
}

public static class FacebookConstant
{
    public static readonly Dictionary<FacebookAttachmentType, List<string>> AcceptedAttachmentExtensions =
        new ()
        {
            { FacebookAttachmentType.Image, ["jpg", "jpeg", "png", "gif"] },
            { FacebookAttachmentType.Video, ["mp4", "ogg", "avi", "mov", "webm"] },
            { FacebookAttachmentType.Audio, ["mp3", "wav", "aac", "m4a"] },
            { FacebookAttachmentType.File, ["doc", "docx", "xls", "xlsx", "ppt", "pptx", "pdf", "txt", "json", "html", "xml"] }
        };
    
    public static FacebookAttachmentType GetAttachmentType(string extension)
    {
        extension = extension.TrimStart('.');
        
        foreach (var (key, value) in AcceptedAttachmentExtensions)
        {
            if (value.Contains(extension))
                return key;
        }
        
        throw new AppException("Unsupported attachment type");
    }
}
