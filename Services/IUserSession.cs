using MetanetA_MobileApp.Model;

namespace MetanetA_MobileApp.Services;

public interface IUserSession
{
    UserInfo? CurrentUser { get; set; }

    bool IsAuthenticated { get; }

    void SignOut();
}
