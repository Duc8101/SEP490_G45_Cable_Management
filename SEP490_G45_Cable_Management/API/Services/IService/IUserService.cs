using Common.Base;
using Common.DTO.UserDTO;
using Common.Pagination;

namespace API.Services.IService
{
    public interface IUserService
    {
        Task<ResponseBase<TokenDTO?>> Login(LoginDTO DTO);
        Task<ResponseBase<bool>> Create(UserCreateDTO DTO);
        Task<ResponseBase<bool>> ForgotPassword(ForgotPasswordDTO DTO);
        Task<ResponseBase<bool>> ChangePassword(ChangePasswordDTO DTO, string email);
        Task<ResponseBase<Pagination<UserListDTO>?>> ListPaged(string? filter, int page);
        Task<ResponseBase<bool>> Update(Guid UserID, UserUpdateDTO DTO);
        Task<ResponseBase<bool>> Delete(Guid UserID, Guid UserLoginID);
        Task<ResponseBase<List<UserListDTO>?>> ListWarehouseKeeper();
    }
}
