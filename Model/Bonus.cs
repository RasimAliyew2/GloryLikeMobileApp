using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetanetA_MobileApp.Services.Abstractions;

namespace MetanetA_MobileApp.Model
{
    public class Bonus : IBonus
    {
        public string UniqueCode { get;set; }
        public bool IsUsed { get; set; }
        public ProductTypeAndBonus ProductTypeBonus { get; set; }
        public string ProductCode { get; set; }
    }
}
