namespace AChat.Domain;

public class StringValueAttribute(string value) : Attribute
{
    public string StringValue { get; protected set; } = value;
}

#region Enum extensions
public static class EnumExtensions
{
    public static string ToValue(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());

        if (fieldInfo?.GetCustomAttributes(typeof(StringValueAttribute), false) is not StringValueAttribute[] attributes)
            return value.ToString();

        return attributes.Length > 0 ? attributes[0].StringValue : value.ToString();
    }

    public static T ToEnum<T>(this string value) where T : Enum
    {
        var type = typeof(T);
        if (!type.IsEnum) throw new InvalidOperationException();

        foreach (var field in type.GetFields())
        {
            if (field.GetCustomAttributes(typeof(StringValueAttribute), false) is not StringValueAttribute[] attributes)
                continue;

            if (attributes.Length > 0 && attributes[0].StringValue == value)
                return (T)field.GetValue(null)!;
        }

        throw new KeyNotFoundException();
    }
}
#endregion

public enum AppRole
{
    [StringValue("admin")] Admin = 1,
    [StringValue("user")] User = 2
}

public enum SourceType
{
    Gmail = 1,
    Facebook = 2
}

public enum Gender
{
    Male = 1,
    Female = 2,
    Other = 3,
    Unknown = 4
}

public enum FacebookAttachmentType
{
    [StringValue("image")] Image = 1,
    [StringValue("video")] Video = 2,
    [StringValue("audio")] Audio = 3,
    [StringValue("file")] File = 4
}

public enum TemplateType
{
    Email = 1,
    Message = 2
}
