using DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO
{
    public class ResponseDTO<T>
    {
        public bool Success { get; set; }
        public int Code { get; set; }
        public string Message { get; set; } = null!;

        public T? Data { get; set; }

        public ResponseDTO()
        {

        }
        public ResponseDTO(T data, string message, int code)
        {
            Data = data;
            Message = message;
            Code = code;
            Success = true;
        }

        public ResponseDTO(T data)
        {
            Data = data;
            Message = "";
            Code = (int) HttpStatusCode.OK;
            Success = true;
        }

        public ResponseDTO(string message, int code)
        {
            Message = message;
            Code = code;
            Success = false;
        }
    }
}
