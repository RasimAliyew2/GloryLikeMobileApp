using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetanetA_MobileApp.Model;

namespace MetanetA_MobileApp.Services.Abstractions
{
    public interface IBonus
    {
        string UniqueCode { get; set; }
        bool IsUsed { get; set; }

        ProductTypeAndBonus ProductTypeBonus { get; set; }

        string ProductCode { get; set; }
    }
}
