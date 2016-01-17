using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    public interface IFighter
    {
            int Hp { get; }
            int MaxHp { get; }
            bool Dead { get; }
            Stats Stats { get; }

            int CA //Physical attacks
            {
                get;
            }
            int CASpecial //Mental attacks
            {
                get;
            }

            Armor WornArmor { get; }
            Shield EmbracedShield { get; }
            Weapon HandledWepon { get; }

            void SufferDamage(Damage dmg);
            void HealDamage(Damage dmg);

            void DecreaseMaxHp(int deltaHp);
            void IncreaseMaxHp(int deltaHp);
            
            void Attack(Character defenderCharachter);

            void Die(Character killer);

            void WearArmor(Item armor);
            void RemoveArmor();

            void EmbraceShield(Item shield);
            void DisembraceShield();

            void HandleWeapon(Item weapon);
            void UnhandleWeapon();

        
    }
}
