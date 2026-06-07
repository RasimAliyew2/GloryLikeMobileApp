using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetanetA_MobileApp.Services.Abstractions
{
    public interface IQrCode
    {
        public string UniqueCode { get; set; }
        public float Bonus { get; set; }
    }
}
