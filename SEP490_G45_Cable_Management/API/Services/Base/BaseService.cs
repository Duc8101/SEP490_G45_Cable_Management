using AutoMapper;

namespace API.Services.Base
{
    public class BaseService
    {
        private protected readonly IMapper _mapper;
        public BaseService(IMapper mapper)
        {
            _mapper = mapper;
        }
    }
}
