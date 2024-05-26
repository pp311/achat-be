using System.Text.Json.Serialization;
using AChat.Domain;

namespace AChat.Application.ViewModels.Template;

public class CreateTemplateRequest
{
    public string Name { get; set; } = null!;
    public string Content { get; set; } = null!;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TemplateType Type { get; set; }
}