using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    public delegate void Attacked(AttackEventArgs e);

    public interface IFighter : IAttacker
    {
        Armor WornArmor { get; }
        Shield EmbracedShield { get; }
        Weapon HandledWepon { get; }

        void HandleWeapon(Item weapon);
        void UnhandleWeapon();

        event Attacked Attacked;
    }
}
