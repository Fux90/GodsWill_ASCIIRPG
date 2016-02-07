using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.View
{
    public interface IMerchantViewer : IViewer
    {
        void NotifyMerchantName(string merchantName);
        void NotifyBuyerGold(int gold);
        void BringUpAndFocus(Pg interactor);
        void NotifyExcange();
    }
}
