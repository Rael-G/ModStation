namespace ModManager.Core.Exceptions;

public class ModManagerException : Exception
{
    public ModManagerException()
    {
        
    }

    public ModManagerException(string? message) : base(message)
    {
        
    }

    public ModManagerException(string? message, Exception? innerException) : base(message, innerException) 
    {
        
    }
}
