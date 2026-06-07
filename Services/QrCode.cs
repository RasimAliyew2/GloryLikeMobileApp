using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetanetA_MobileApp.Services.Abstractions;

namespace MetanetA_MobileApp.Services
{
    public class QrCode : IQrCode
    {
        public required string UniqueCode { get; set; }
        public required float Bonus { get; set; }
    }
}
