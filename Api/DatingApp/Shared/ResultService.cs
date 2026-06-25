

namespace DatingApp.Shared
{
    public class ResultService <T>
    {
        public bool IsSuccess { get; set; }

        public string? ErrorMessage { get; set; }

        public T? Data { get; set; }

         public static ResultService<T> Success(T data)=>
            new() { IsSuccess = true, Data = data };


         public static ResultService<T> Failuer(string error) =>
            new() { IsSuccess = false, ErrorMessage = error };

    }
}
