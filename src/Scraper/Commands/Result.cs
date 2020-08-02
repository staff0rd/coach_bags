namespace coach_bags_selenium
{
    public class Result<T> : Result
    {
        private readonly T _response;
        public Result(T response)
        {
            _response = response;
        }
        public Result() {}
        public T Response => _response;
    }

    public class Result
    {
        public bool IsError { get; set; }
        public string Message { get; set; }

        public Result() {}
        public Result(string message) {}

        public static Result Ok() => new Result();

        public static Result Error(string message) => new Result { IsError = true, Message = message };   
    }
}