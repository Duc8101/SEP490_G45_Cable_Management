using AutoMapper;

namespace API.Services.Service
{
    public class BaseService
    {
        internal readonly IMapper _mapper;
        public BaseService(IMapper mapper)
        {
            _mapper = mapper;
        }
    }
}
