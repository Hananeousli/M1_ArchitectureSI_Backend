namespace UniversiteDomain.Exceptions.ParcoursExceptions;

public class DuplicateUeDansParcoursException : Exception
{
    public DuplicateUeDansParcoursException() : base() { }
    public DuplicateUeDansParcoursException(string message) : base(message) { }
    public DuplicateUeDansParcoursException(string message, Exception innerException) : base(message, innerException) { }
}