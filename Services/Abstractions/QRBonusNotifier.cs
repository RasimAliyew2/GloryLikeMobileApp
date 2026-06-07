using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetanetA_MobileApp.Model;

namespace MetanetA_MobileApp.Services.Abstractions
{
    public class QRBonusNotifier : IQRBonusNotifier
    {
        public event Action<BonusTransaction> QRBonusAdded;
        public void PublishNewQRBonus(BonusTransaction row)
        {
            QRBonusAdded?.Invoke(row);
        }
    }
}
