namespace DevArena.Shared
{
    public class Result<T>
    {
        public T? Data { get; set; }
        public bool HasError { get; set; }
        public string? Message { get; set; }
        public static Result<T> Success(T data, string? message = null)
        {
            return new Result<T>
            {
                Data = data,
                HasError = false,
                Message = message
            };
        }

        public static Result<T> Failure(string message)
        {
            return new Result<T>
            {
                HasError = true,
                Message = message
            };
        }
    }
}
