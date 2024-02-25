using AutoMapper;

namespace API.Services.Service
{
    public class BaseService
    {
        internal readonly IMapper mapper;
        public BaseService(IMapper mapper)
        {
            this.mapper = mapper;
        }
    }
}
