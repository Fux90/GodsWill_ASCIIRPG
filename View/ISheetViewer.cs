using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.Model.Gods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.View
{
    public interface ISheetViewer : IViewer
    {
        void NotifyName(string name);
        void NotifyLevel(Pg.Level level, God god);
        void NotifyXp(int currentXp, int nextXp);
        void NotifyGold(int currentGold);
        void NotifyHp(int currentHp, int maximumHp);
        void NotifyHunger(int hunger);
        void NotifyDefences(int CA, int SpecialCA);
        void NotifyArmor(Armor armor);
        void NotifyShield(Shield shield);
        void NotifyWeapon(Weapon weapon);
        void NotifyStat(StatsType stat, int value); 
    }
}
