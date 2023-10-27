using DataAccess.DTO.UserDTO;
using DataAccess.DTO;

namespace API.Services.IService
{
    public interface IUserService
    {
        Task<ResponseDTO<TokenDTO?>> Login(LoginDTO DTO);
        Task<ResponseDTO<bool>> Create(UserCreateDTO DTO);
        Task<ResponseDTO<bool>> ForgotPassword(ForgotPasswordDTO DTO);
        Task<ResponseDTO<bool>> ChangePassword(ChangePasswordDTO DTO, string email);
        Task<ResponseDTO<PagedResultDTO<UserListDTO>?>> ListPaged(string? filter, int page);
        Task<ResponseDTO<bool>> Update(Guid UserID, UserUpdateDTO DTO);
        Task<ResponseDTO<bool>> Delete(Guid UserID, Guid UserLoginID);
        Task<ResponseDTO<List<UserListDTO>?>> ListWarehouseKeeper();
    }
}
