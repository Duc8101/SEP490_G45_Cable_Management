using Common.Base;
using Common.DTO.UserDTO;

namespace API.Services.Users
{
    public interface IUserService
    {
        ResponseBase Login(LoginDTO DTO);
        Task<ResponseBase> Create(UserCreateDTO DTO);
        Task<ResponseBase> ForgotPassword(ForgotPasswordDTO DTO);
        ResponseBase ChangePassword(ChangePasswordDTO DTO, string email);
        ResponseBase ListUserPaged(string? filter, int page);
        ResponseBase Update(Guid userId, UserUpdateDTO DTO);
        ResponseBase Delete(Guid userId, Guid userLoginId);
        ResponseBase ListWarehouseKeeper();
    }
}
