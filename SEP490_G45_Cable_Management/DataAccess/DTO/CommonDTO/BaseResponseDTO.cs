using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.CommonDTO
{
    public class BaseResponseDTO<T>
    {
        public bool Success { get; set; }
        public int Code { get; set; }
        public string Message { get; set; } = null!;
        public MetaDataDTO? MetaData { get; set; }

        public T? Data { get; set; }
        public BaseResponseDTO(T data, string message, int code)
        {
            Data = data;
            Message = message;
            Code = code;
            Success = true;
        }

        public BaseResponseDTO(string message, int code)
        {
            Message = message;
            Code = code;
            Success = false;
        }
    }
}
