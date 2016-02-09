using GodsWill_ASCIIRPG.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Traps
{
    [Serializable]
    public abstract class Trap : HiddenAtom, ITriggerable, IAttacker, ISerializable
    {
        const string chargeSerializationName = "charge";
        const string bonusSerializationName = "bonus";
        const string damageSerializationName = "damage";

        public const int UnlimitedCharge = -1;

        public int Charge { get; private set; }
        public int Bonus { get; private set; }
        public DamageCalculator Damage { get; private set;}

        public Trap(int charge,
                    int bonus,
                    DamageCalculator damage,
                    string description = "Basic Trap",
                    Coord position = new Coord())
            : base("Trap",
                  "^",
                  System.Drawing.Color.Blue,
                  true,
                  false,
                  description,
                  position,
                  true)
        {
            Charge = charge;
            Bonus = bonus;
            Damage = damage;
        }

        public Trap(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Charge = (int)info.GetValue(chargeSerializationName, typeof(int));
            Bonus = (int)info.GetValue(bonusSerializationName, typeof(int));
            Damage = (DamageCalculator)info.GetValue(damageSerializationName, typeof(DamageCalculator));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(chargeSerializationName, Charge , typeof(int));
            info.AddValue(bonusSerializationName, Bonus, typeof(int));
            info.AddValue(damageSerializationName, Damage, typeof(DamageCalculator));
        }

        public bool ImmediateTrigger
        {
            get
            {
                return true;
            }
        }

        public Atom TriggeringSubject
        {
            get;
            private set;
        }

        public virtual void Attack(IDamageable defenderCharachter)
        {
            var msg = new StringBuilder();

            msg.AppendFormat("Triggered against {0}: ", ((Atom)defenderCharachter).Name);
            var tpc = Dice.Throws(20) + Bonus;

            var defenderCA = defenderCharachter.CA;
            if (tpc >= defenderCA)
            {
                var actualDmg = Damage.CalculateDamage();

                msg.AppendFormat("HIT! ({0} vs. {1})", tpc, defenderCA);
                NotifyListeners(msg.ToString());
                defenderCharachter.SufferDamage(actualDmg);
            }
            else
            {
                msg.AppendFormat("MISSED... ({0} vs. {1})", tpc, defenderCA);
                NotifyListeners(msg.ToString());
            }
        }

        public override bool Interaction(Atom interactor)
        {
            if(!Hidden)
            {
                var name = this.Name;
                var n = name[0].IsVowel() ? "n" : "";
                interactor.NotifyListeners(String.Format("Mmm... It's a{0} {1}", n, name));
            }

            return false;
        }

        public void RegisterTriggeringSubject(Atom triggeringSubject)
        {
            TriggeringSubject = triggeringSubject;
        }

        public virtual void Trigger()
        {
            Show();

            var unlimited = Charge == Trap.UnlimitedCharge;

            if (unlimited || Charge > 0)
            {
                if (typeof(IDamageable).IsAssignableFrom(TriggeringSubject.GetType()))
                {
                    Attack((IDamageable)TriggeringSubject);
                    if(!unlimited)
                    {
                        Charge--;
                    }
                }
            }
        }
    }
}
