using MetanetA_MobileApp.Model;

namespace MetanetA_MobileApp.Services;

public class UserSession : IUserSession
{
    public UserInfo? CurrentUser { get; set; }

    public bool IsAuthenticated => CurrentUser is not null && CurrentUser.Id > 0;

    public void SignOut()
    {
        CurrentUser = null;
    }
}
