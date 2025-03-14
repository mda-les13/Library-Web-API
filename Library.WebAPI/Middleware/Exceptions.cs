using System.Net;

namespace Library.WebAPI.Middleware
{
    public abstract class CustomException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        protected CustomException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public class ValidationFailedException : CustomException
    {
        public ValidationFailedException(IEnumerable<string> errors)
            : base("Validation Failed", HttpStatusCode.BadRequest)
        {
            Errors = errors;
        }

        public IEnumerable<string> Errors { get; }
    }

    public class NotFoundException : CustomException
    {
        public NotFoundException(string message)
            : base(message, HttpStatusCode.NotFound)
        {
        }
    }

    public class BadRequestException : CustomException
    {
        public BadRequestException(string message)
            : base(message, HttpStatusCode.BadRequest)
        {
        }
    }

    public class InternalServerException : CustomException
    {
        public InternalServerException(string message)
            : base(message, HttpStatusCode.InternalServerError)
        {
        }
    }

    public class ConflictException : CustomException
    {
        public ConflictException(string message)
            : base(message, HttpStatusCode.Conflict)
        {
        }
    }

    public class ForbiddenException : CustomException
    {
        public ForbiddenException(string message)
            : base(message, HttpStatusCode.Forbidden)
        {
        }
    }

    public class UnprocessableEntityException : CustomException
    {
        public UnprocessableEntityException(string message)
            : base(message, HttpStatusCode.UnprocessableEntity)
        {
        }
    }
}
