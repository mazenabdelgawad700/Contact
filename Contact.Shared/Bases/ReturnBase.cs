using System.Net;

namespace Contact.Shared.Bases
{
    public class ReturnBase
    {
        public ReturnBase<T> Success<T>(T entity, string? message = null)
        {
            return new ReturnBase<T>()
            {
                Data = entity,
                StatusCode = HttpStatusCode.OK,
                Succeeded = true,
                Message = message ?? "Success",
            };
        }
        public ReturnBase<T> Failed<T>(string? message = null)
        {
            return new ReturnBase<T>()
            {
                StatusCode = HttpStatusCode.ExpectationFailed,
                Succeeded = false,
                Message = message ?? "Operation Failed"
            };
        }
    }
    public class ReturnBase<T>
    {
        public ReturnBase()
        {
            Succeeded = true;
            Errors = new List<string>();
            Message = "";
        }
        public ReturnBase(T data, string? message = null)
        {
            Succeeded = true;
            Message = message!;
            Data = data;
            Errors = new List<string>();
        }
        public ReturnBase(T data, string message, bool succeeded)
        {
            Succeeded = succeeded;
            Message = message;
            Errors = new List<string>();
            Data = data;
        }
        public HttpStatusCode StatusCode { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        public T Data { get; set; }
    }
}
