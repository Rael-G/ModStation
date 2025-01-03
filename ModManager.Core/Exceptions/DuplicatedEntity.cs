namespace ModManager.Core.Exceptions;

public class DuplicatedEntity : ModManagerException
{
    public DuplicatedEntity()
    {
        
    }

    public DuplicatedEntity(string? message) : base(message)
    {
        
    }

    public DuplicatedEntity(string? message, Exception? innerException) : base(message, innerException) 
    {
        
    }
}