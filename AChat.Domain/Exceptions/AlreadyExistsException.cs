namespace AChat.Domain.Exceptions;

public class AlreadyExistsException : Exception
{
	public AlreadyExistsException(string entityName, string? identifier = null)
	: base($"Entity \"{entityName}\" ({identifier}) already exists.")
	{
	}

	public static void ThrowIfTrue<T>(bool isEntityExists, string? identifier = null)
	{
		if (isEntityExists)
			throw new AlreadyExistsException(typeof(T).Name, identifier);
	}	
}