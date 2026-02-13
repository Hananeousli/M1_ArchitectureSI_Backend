namespace UniversiteDomain.Exceptions.NoteExceptions;

public class InvalidNoteValueException : Exception
{
    public InvalidNoteValueException(float noteValeur) : base() { }
    public InvalidNoteValueException(string message) : base(message) { }
    public InvalidNoteValueException(string message, Exception innerException) : base(message, innerException) { }
}