using AChat.Domain;

namespace AChat.Application.ViewModels.Template;

public class TemplateResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Content { get; set; } = null!;
    public TemplateType Type { get; set; }
}