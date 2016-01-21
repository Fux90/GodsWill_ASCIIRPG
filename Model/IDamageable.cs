using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    public interface IDamageable : IStatsProvided
    {
        int Hp { get; }
        int MaxHp { get; }
        bool Dead { get; }

        int CA //Physical attacks
        {
            get;
        }
        int CASpecial //Mental attacks
        {
            get;
        }

        void SufferDamage(Damage dmg);
        void HealDamage(Damage dmg);

        void DecreaseMaxHp(int deltaHp);
        void IncreaseMaxHp(int deltaHp);

        void Die(IFighter killer);

        void WearArmor(Item armor);
        void RemoveArmor();

        void EmbraceShield(Item shield);
        void DisembraceShield();
    }
}
