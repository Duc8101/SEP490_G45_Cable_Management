using System.Net;

namespace Common.Base
{
    public class ResponseBase<T>
    {
        public bool Success { get; set; }
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }

        public ResponseBase()
        {

        }
        public ResponseBase(T data, string message, int code)
        {
            Data = data;
            Message = message;
            Code = code;
            Success = false;
        }

        public ResponseBase(string message, int code)
        {
            Message = message;
            Code = code;
            Success = false;
        }

        public ResponseBase(T data, string message)
        {
            Data = data;
            Message = message;
            Code = (int)HttpStatusCode.OK;
            Success = true;
        }

        public ResponseBase(T data)
        {
            Data = data;
            Code = (int)HttpStatusCode.OK;
            Success = true;
        }

    }

    public class ResponseBase : ResponseBase<object>
    {
        public ResponseBase(object data, string message, int code) : base(data, message, code)
        {

        }

        public ResponseBase(string message, int code) : base(message, code)
        {

        }

        public ResponseBase(object data, string message) : base(data, message)
        {

        }

        public ResponseBase(object data) : base(data)
        {

        }
    }

}
