using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetanetA_MobileApp.Model;

namespace MetanetA_MobileApp.Services.Abstractions
{
    public interface IGiftPurchaseNotifier
    {
        event Action<BonusTransaction> GiftPurchaseAdded;

        void PublishNewGiftPurchase(BonusTransaction row);
    }
}
