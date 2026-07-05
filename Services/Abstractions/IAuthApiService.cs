using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services.GetDataFromServer;

namespace MetanetA_MobileApp.Services.Abstractions;

public interface IAuthApiService
{
    Task<AuthApiResult<AuthUserDto>> RegisterAsync(AuthRegisterRequest request);

    Task<AuthApiResult<AuthUserDto>> LoginAsync(AuthLoginRequest request);

    Task<AuthApiResult<ForgotPasswordDto>> ForgotPasswordAsync(ForgotPasswordRequest request);

    Task<AuthApiResult<object?>> ResetPasswordAsync(ResetPasswordRequest request);
}
