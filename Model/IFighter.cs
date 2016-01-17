using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    public interface IFighter
    {
            Armor WornArmor { get; }
            Shield EmbracedShield { get; }
            Weapon HandledWepon { get; }

            void Attack(IDamageable defenderCharachter);

            void HandleWeapon(Item weapon);
            void UnhandleWeapon();
    }
}
