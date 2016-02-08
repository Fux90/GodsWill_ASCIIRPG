using GodsWill_ASCIIRPG.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace GodsWill_ASCIIRPG
{
    [Serializable]
	public class Damage : ISerializable
	{
        private const string dmgsSerializableName = "dmgs";

        private const int minimumDamage = 1;

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
                return Math.Max(minimumDamage, totDmg);
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

        public Damage(SerializationInfo info, StreamingContext context)
        {
            dmgs = (Dictionary<DamageType, int>)info.GetValue(dmgsSerializableName, typeof(Dictionary<DamageType, int>));
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

        public string ToHorString()
        {
            var str = new StringBuilder();
            var sep = "";
            foreach (var dmgType in dmgs.Keys)
            {
                var dmg = dmgs[dmgType];
                if (dmg > 0)
                {
                    str.AppendFormat("{0}{1}[{2}]",
                                        sep,
                                        dmg,
                                        dmgType);
                    sep = "+";
                }
            }
            return str.ToString();
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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(dmgsSerializableName, dmgs, typeof(Dictionary<DamageType, int>));
        }
    }

    [Serializable]
    public class DamageCalculator : ISerializable
    {
        #region CONST_SERIALIZABLE_NAMES
        const string _damagesSerializableName = "damagesCalculations";
        #endregion

        [Serializable]
        public delegate int DamageCalculatorMethod(ThrowModifier mod = ThrowModifier.Normal);
        Dictionary<DamageType, DamageCalculatorMethod> dmgs;

        public DamageCalculatorMethod this[DamageType dmgType]
        {
            get
            {
                return dmgs.ContainsKey(dmgType) ? dmgs[dmgType] : (mod) => 0;
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

        public DamageCalculator(SerializationInfo info, StreamingContext context)
            : this(new Dictionary<DamageType, DamageCalculatorMethod>())
        {
            var a = 0;
            a++;

            dmgs = (Dictionary<DamageType, DamageCalculatorMethod>)info.GetValue(_damagesSerializableName, typeof(Dictionary<DamageType, DamageCalculatorMethod>));
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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(_damagesSerializableName, dmgs, typeof(Dictionary<DamageType, DamageCalculatorMethod>));
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            foreach (var dmgType in dmgs.Keys)
            {
                var min = dmgs[dmgType](ThrowModifier.Minimized);
                var max = dmgs[dmgType](ThrowModifier.Maximized);
                var maxStr = min == max ? "" : String.Format(" - {0}", max);
                str.AppendLine(String.Format("{0}: {1}{2}", dmgType, min, maxStr));
            }
            return str.ToString();
        }
    }
}