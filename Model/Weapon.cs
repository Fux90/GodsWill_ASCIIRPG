using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.Model.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GodsWill_ASCIIRPG
{
    [Serializable]
	public abstract class Weapon : Item
	{
        public static readonly _SpecialAttack NoSpecialAttack = (attacker, defender) => { };
        public static readonly string DefaultSymbol = "/";

        public abstract int BonusOnTPC { get; }

        private class _UnarmedAttack : Weapon
        {
            public override int BonusOnTPC { get { return 0; } }

            public _UnarmedAttack()
                : base("Unarmed Attack",
                      "p",
                      Color.White,
                      new DamageCalculator( new Dictionary<DamageType, 
                                            DamageCalculator.DamageCalculatorMethod>(){ { DamageType.Physical, () => 1 } }),
                      null,
                      Weapon.NoSpecialAttack,
                      "A punch, a bite, whatever creature can do without weapons",
                      new Coord())
            {
                IsUnarmed = true;
            }
        }

        private static readonly _UnarmedAttack unarmedAttack = new _UnarmedAttack();
        public static Weapon UnarmedAttack
        {
            get
            {
                return unarmedAttack;
            }
        }

        private DamageCalculator damage;
        public Damage Damage { get { return damage.CalculateDamage(); } }

        public bool IsUnarmed { get; protected set; }

        public delegate void _SpecialAttack(IFighter attacker, IDamageable defender);
        private _SpecialAttack specialAttack;
        public _SpecialAttack SpecialAttack
        {
            get
            {
                return specialAttack == null
                ? Weapon.NoSpecialAttack
                : specialAttack;
            }
        }

        private string specialAttackDescription;
        public string SpecialAttackDescription
        {
            get
            {
                this.ConsumeUse();
                return specialAttackDescription == null
                ? ""
                : specialAttackDescription;
            }
        }

        public bool HasSpecialAttack { get { return specialAttack != null; } }

        public bool SpecialAttackActivated { get; private set; }

        public Weapon(  string name,
                        string symbol,
                        Color color,
                        DamageCalculator damage,
                        string specialAttackDescription = null,
                        _SpecialAttack specialAttack = null,
                        string description = "Base weapon of the game",
                        Coord position = new Coord(),
                        int cost = 0,
                        int weight = 1,
                        int uses = Item._UnlimitedUses)
            : base(name, symbol, color, true, false, description, position, cost, weight, uses)
        {
            this.damage = damage;
            this.specialAttack = specialAttack;
            this.specialAttackDescription = specialAttackDescription;
        }

        public Weapon(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        public void ActivateSpecialAttack()
        {
            SpecialAttackActivated = true;
        }

        public void DectivateSpecialAttack()
        {
            SpecialAttackActivated = false;
        }

        public override string FullDescription
        {
            get
            {
                var str = new StringBuilder();
                str.AppendLine(String.Format("{0} [{1}]", Name, ItemTypeName));
                str.AppendLine(String.Format("Cost: {0} Weight{1}",
                                             Cost,
                                             Weight));
                str.AppendLine();
                str.AppendLine("Qui i danni normali");
                if (HasSpecialAttack)
                {
                    str.AppendLine();
                    str.AppendLine("Special:");
                    str.AppendLine(SpecialAttackDescription);
                    str.AppendLine(String.Format("{0}", SpecialAttackActivated ? "Active" : "Disactive"));
                }
                str.AppendLine();
                str.AppendLine(Description);
                return str.ToString();
            }
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            str.AppendLine(String.Format("{0}[{1}]",
                                         Name,
                                         ItemTypeName));
            str.AppendLine(String.Format("Bonus: {0}", BonusOnTPC));
            str.AppendLine(Damage.ToString());
            return str.ToString();
        }
    }
}