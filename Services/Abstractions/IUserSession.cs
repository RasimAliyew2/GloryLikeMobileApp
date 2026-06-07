using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetanetA_MobileApp.Model;

namespace MetanetA_MobileApp.Services.Abstractions
{
    public interface IUserSession
    {
        UserInfo? CurrentUser { get; set; }
        string? PhoneNumber { get; set; }
        string? OtpCode { get; set; }

        int SelectedBottomMenuItem { get; set; }
    }
}
