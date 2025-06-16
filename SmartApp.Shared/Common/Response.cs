using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApp.Shared.Common
{
    public class Response<T>
    {
        public bool isSuccess { get; set; }
        public string message { get; set; } = string.Empty;
        public T data { get; set; }

        public static Response<T> SuccessResponse(T _data, string _message = "") =>
            new Response<T> { isSuccess = true, message = _message, data = _data };

        public static Response<T> Failure(string _message) =>
            new Response<T> { isSuccess = false, message = _message, data = default };
    }
}
