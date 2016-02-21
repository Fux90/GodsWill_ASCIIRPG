using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.Model.Core;
using GodsWill_ASCIIRPG.Model.Items;
using GodsWill_ASCIIRPG.Model.Weapons;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GodsWill_ASCIIRPG
{
    public class WeaponBuilder : ItemGenerator<Weapon>
    {
        public override Weapon GenerateTypedRandom(Pg.Level level, Coord position)
        {
            var weaponsType = typeof(Weapon);

            // Requested power
            var allWeapons = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(s => s.GetTypes())
                            .Where(w => weaponsType.IsAssignableFrom(w)
                                    && !w.IsNestedPrivate
                                    && !w.IsAbstract)
                            //.Where(w =>
                            //{
                            //    var attribute = w.GetCustomAttributes(typeof(Prerequisite), false).FirstOrDefault();
                            //    return attribute != null && ((Prerequisite)attribute).MinimumLevel == level;
                            //})
                            .ToArray();
            var weapons = allWeapons.Where(w => 
                                    {
                                        var attribute = w.GetCustomAttributes(typeof(Prerequisite), false).FirstOrDefault();
                                        return attribute != null && ((Prerequisite)attribute).MinimumLevel == level;
                                    })
                                    .ToArray();

            // Lesser power
            if (weapons.Length == 0)
            {
                weapons =   allWeapons
                            .Where(w =>
                            {
                                var attribute = w.GetCustomAttributes(typeof(Prerequisite), false).FirstOrDefault();
                                return attribute != null && ((Prerequisite)attribute).MinimumLevel < level;
                            })
                            .ToArray();
            }
            // No prerequisite
            if (weapons.Length == 0)
            {
                weapons =   allWeapons
                            .Where(w => w.GetCustomAttributes(typeof(Prerequisite), false).Count() == 0)
                            .ToArray();
            }

            if (weapons.Length == 0)
            {
                throw new Exception("Unexpected No Weapon");
            }

            var ix = Dice.Throws(new Dice(weapons.Length)) - 1;

            var abstractGeneratorType = typeof(ItemGenerator<>).MakeGenericType(new Type[] { weapons[ix] });
            var generatorType = AppDomain.CurrentDomain.GetAssemblies()
                                                       .SelectMany(s => s.GetTypes())
                                                       .Where(wg => abstractGeneratorType.IsAssignableFrom(wg)
                                                               && !wg.IsNestedPrivate
                                                               && !wg.IsAbstract).FirstOrDefault();
            if(generatorType == null)
            {
                throw new Exception("Unexpected No Weapon Generator");
            }

            return (Weapon)((ItemGenerator)Activator.CreateInstance(generatorType)).GenerateRandom(level, position);
        }
    }

    [Serializable]
    [RandomGenerable]
    public abstract class Weapon : Item, ISerializable
	{
        #region CONST_SERIALIZATION_NAMES

        const string _damageSerializableName = "damage";
        const string _specialAttackSerializationName = "specialAttack";

        #endregion

        public partial class WeaponSpecialAttacks
        {
            public static _SpecialAttack NoSpecialAttack
            {
                get
                {
                    return (attacker, defender) => { };
                }
            }

            [WeaponDescription("A magical flame inflicts d6 damage.")]
            public static _SpecialAttack Flaming
            {
                get
                {
                    return (attacker, defender) => 
                    {
                        var dmgAmount = Dice.Throws(1, 6);

                        var dmg = new Damage(new Dictionary<DamageType, int>()
                        {
                            { DamageType.Fire, dmgAmount}
                        });

                        defender.SufferDamage(dmg);
                    };
                }
            }
        }

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
                                            DamageCalculator.DamageCalculatorMethod>(){ { DamageType.Physical, (mod) => 1 } }),
                      null,
                      WeaponSpecialAttacks.NoSpecialAttack,
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
                this.ConsumeUse();
                return specialAttack == null
                ? WeaponSpecialAttacks.NoSpecialAttack
                : specialAttack;
            }
        }

        private string specialAttackDescription;
        public string SpecialAttackDescription
        {
            get
            {
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
            damage = (DamageCalculator)info.GetValue(   _damageSerializableName, 
                                                        typeof(DamageCalculator));
            specialAttack = (_SpecialAttack)info.GetValue(  _specialAttackSerializationName,
                                                            typeof(_SpecialAttack));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(  _damageSerializableName, 
                            damage, 
                            typeof(DamageCalculator));
            info.AddValue(  _specialAttackSerializationName, 
                            specialAttack, 
                            typeof(_SpecialAttack));
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
                str.AppendLine(damage.ToString());
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
            str.AppendLine(String.Format("{0}[{1}]{2}",
                                         Name,
                                         ItemTypeName,
                                         SpecialAttackActivated ? "*" : ""));
            str.AppendLine(String.Format("Bonus: {0}", BonusOnTPC));
            str.AppendLine(damage.ToString()); //Damage calculates random damage
            str.AppendLine(SpecialAttackDescription);
            return str.ToString();
        }
    }
}