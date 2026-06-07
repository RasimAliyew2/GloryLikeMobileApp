using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetanetA_MobileApp.Model;

namespace MetanetA_MobileApp.Services.Abstractions
{
    public interface IQRBonusNotifier
    {
        event Action<BonusTransaction> QRBonusAdded;       
        void PublishNewQRBonus(BonusTransaction row);
        
    }
}

