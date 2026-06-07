using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetanetA_MobileApp.Services
{
    public static class AdjustUserInfo
    {
        public static string AdjustPhoneNumber(string PhoneNumber)
        {
            if (!string.IsNullOrWhiteSpace(PhoneNumber))
            {
                PhoneNumber = PhoneNumber.Trim();

                if (PhoneNumber.StartsWith("0"))
                {
                    PhoneNumber = "994" + PhoneNumber.Substring(1);
                }
                else if (!PhoneNumber.StartsWith("994"))
                {
                    PhoneNumber = "994" + PhoneNumber;
                }
            }
            return PhoneNumber;
        }
    }
}
