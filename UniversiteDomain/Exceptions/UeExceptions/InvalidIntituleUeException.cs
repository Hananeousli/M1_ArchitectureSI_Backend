namespace UniversiteDomain.Exceptions.UeExceptions;

public class InvalidIntituleUeException : Exception
{
    public InvalidIntituleUeException() : base() { }
    public InvalidIntituleUeException(string message) : base(message) { }
    public InvalidIntituleUeException(string message, Exception innerException) : base(message, innerException) { }
}