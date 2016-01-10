using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodsWill_ASCIIRPG
{
	public class Damage
	{
        Dictionary<DamageType, int> dmgs;

        public int this[DamageType dmgType]
        {
            get
            {
                return dmgs.ContainsKey(dmgType) ? dmgs[dmgType] : 0;
            }

            set
            {
                dmgs[dmgType] = value;
            }
        }
        public static DamageType[] AllDamageTypes { get { return (DamageType[])Enum.GetValues(typeof(DamageType)); } }
        public int TotalDamage
        {
            get
            {
                var totDmg = 0;
                foreach (var dmg in dmgs.Values)
                {
                    totDmg += dmg;
                }
                return totDmg;
            }
        }

        public Damage(Dictionary<DamageType, int> dmgs)
        {
            this.dmgs = dmgs;
        }

        public Damage()
            : this(new Dictionary<DamageType, int>())
        {
            
        }

        public static Damage operator +(Damage dmg1, Damage dmg2)
        {
            var dmgs = new Dictionary<DamageType, int>();
            var dmgTypes = Damage.AllDamageTypes;
            foreach (var dmgType in dmgTypes)
            {
                dmgs[dmgType] = dmg1[dmgType] + dmg2[dmgType];
            }

            return new Damage(dmgs);
        }

        public static Damage operator -(Damage dmg1, Damage dmg2)
        {
            var dmgs = new Dictionary<DamageType, int>();
            var dmgTypes = Damage.AllDamageTypes;
            foreach (var dmgType in dmgTypes)
            {
                dmgs[dmgType] = Math.Max(0, dmg1[dmgType] - dmg2[dmgType]);
            }

            return new Damage(dmgs);
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            foreach (var dmgType in dmgs.Keys)
            {
                str.AppendLine(String.Format("{0}: {1}",
                                dmgType,
                                dmgs[dmgType])); 
            }
            return str.ToString();
        }
    }

    public class DamageCalculator
    {
        public delegate int DamageCalculatorMethod();
        Dictionary<DamageType, DamageCalculatorMethod> dmgs;

        public DamageCalculatorMethod this[DamageType dmgType]
        {
            get
            {
                return dmgs.ContainsKey(dmgType) ? dmgs[dmgType] : () => 0;
            }

            set
            {
                dmgs[dmgType] = value;
            }
        }

        public DamageCalculator(Dictionary<DamageType, DamageCalculatorMethod> dmgs)
        {
            this.dmgs = dmgs;
        }

        public DamageCalculator()
            : this(new Dictionary<DamageType, DamageCalculatorMethod>())
        {

        }

        public Damage CalculateDamage()
        {
            var dm = new Dictionary<DamageType, int>();
            foreach (var dmgType in dmgs.Keys)
            {
                dm[dmgType] = dmgs[dmgType]();
            }

            return new Damage(dm);
        } 
    }
}