namespace ConectaMasPro.Application.Common
{
    public class Result
    {
        public bool Success { get; }
        public string? Message { get; set; }

        protected Result(bool success, string? message)
        {
            Success = success;
            Message = message;
        }

        public static Result Ok(string? message = null) => new(true, message);

        public static Result Fail(string error) => new(false, error);
    }

    public class Result<T> : Result
    {
        public T? Data { get; }

        private Result(bool success, T? data, string? error)
            : base(success, error)
        {
            Data = data;
        }

        public static Result<T> Ok(T data, string? message = null) => new (true, data, message);

        public static new Result<T> Fail(string error) => new(false, default, error);
    }
}
