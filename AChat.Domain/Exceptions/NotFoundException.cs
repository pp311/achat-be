namespace AChat.Domain.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string entityName, string? identifier = null)
        : base($"Entity \"{entityName}\" ({identifier}) was not found.")
    {
    }

    /// <summary>
    /// Throws an EntityNotFoundException if the entity is null
    /// </summary>
    public static void ThrowIfNull<T>(T entity, int entityId)
    {
        if (entity == null)
            throw new NotFoundException(typeof(T).Name, entityId.ToString());
    }

    public static void ThrowIfNull<T>(T entity, string? identifier)
    {
        if (entity == null)
            throw new NotFoundException(typeof(T).Name, identifier);
    }

    public static void ThrowIfFalse<T>(bool isEntityExists, int? entityId = null)
    {
        if (!isEntityExists)
            throw new NotFoundException(typeof(T).Name, entityId?.ToString());
    }
}