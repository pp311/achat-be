namespace AChat.Domain.Exceptions;

public class AuthException : Exception
{
    public AuthException() : base("Invalid credentials.")
    {
    }
    
    public static void ThrowIfFalse(bool isValid)
    {
        if (!isValid)
            throw new AuthException();
    }
}
