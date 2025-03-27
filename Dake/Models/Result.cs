namespace Dake.Models
{
    public class Result<T> where T : class
    {
        public Result(bool isSuccess, T data)
        {
            IsSuccess = isSuccess;
            Data = data;
        }

        public bool IsSuccess { get; set; }

        public T Data { get; set; }
    }
}
