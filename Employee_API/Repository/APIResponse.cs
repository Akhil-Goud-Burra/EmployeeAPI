using System.Net;

namespace Employee_API.Repository
{
    public class APIResponse
    {
        public HttpStatusCode StatusCode { get; set; }

        public bool IsSuccess { get; set; }

        public string? InputValidationError { get; set; }

        public string? ErrorMessage { get; set; }

        public string? ThrowedException { get; set; }

        public object? Result { get; set; }
    }
}
