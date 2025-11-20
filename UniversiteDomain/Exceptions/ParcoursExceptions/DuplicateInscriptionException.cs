using System;

namespace UniversiteDomain.Exceptions.ParcoursExceptions 
{
    public class DuplicateInscriptionException : Exception
    {
        public DuplicateInscriptionException() : base()
        {
        }

        public DuplicateInscriptionException(string message) : base(message)
        {
        }

        public DuplicateInscriptionException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}