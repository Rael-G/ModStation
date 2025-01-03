namespace ModManager.Core.Exceptions;

public class DuplicatedEntityException : ModManagerException
{
    public DuplicatedEntityException()
    {
        
    }

    public DuplicatedEntityException(string? message) : base(message)
    {
        
    }

    public DuplicatedEntityException(string? message, Exception? innerException) : base(message, innerException) 
    {
        
    }
}