using GodsWill_ASCIIRPG.Model.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    public interface IMerchant
    {
        void PickUpGold(Gold gold);
        bool GiveAwayGold(int amount, out Gold gold);
    }
}
