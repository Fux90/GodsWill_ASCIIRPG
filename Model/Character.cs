using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.Model.Core;
using GodsWill_ASCIIRPG.View;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace GodsWill_ASCIIRPG
{
    [StraightSightNeededForPerception]
    public abstract class Character : MoveableAtom
    {
        protected enum HpType
        {
            Current,
            Max
        }

        int[] hp;
        int hunger;
        Armor wornArmor;
        Shield embracedShield;
        Weapon handledWeapon;
        Backpack backpack;
        Stats stats;

        protected List<ISheetViewer> CharacterSheets { get; private set; }

        public abstract Dice HealthDice { get; }
        public abstract Dice HungerDice { get; }

        public int Hp { get { return hp[(int)HpType.Current]; } }
        public int MaxHp { get { return hp[(int)HpType.Max]; } }
        public int Hunger { get { return hunger; } }
        public bool Dead { get { return hp[(int)HpType.Current] <= 0 || hunger <= 0 || stats.OneIsLessThanZero(); } }
        public Stats Stats { get { return stats; } }

        public int CA //Physical attacks
        {
            get
            {
                return 10 
                        + EmbracedShield.BonusCA 
                        + Stats[StatsType.Agility].ModifierOfStat()
                        + TempModifiers.GetBonus<int>(  TemporaryModifier.ModFor.CA,
                                                        (a,b) => a + b);
            }
        }

        public int CASpecial //Mental attacks
        {
            get
            {
                return 10 
                        + EmbracedShield.BonusSpecialCA 
                        + Stats[StatsType.Mental]
                        + TempModifiers.GetBonus<int>(TemporaryModifier.ModFor.CASpecial,
                                                        (a, b) => a + b);
            }
        }

        public Armor WornArmor { get { return wornArmor == null ? Armor.Skin : wornArmor; } }
        public Shield EmbracedShield { get { return embracedShield == null ? Shield.NoShield : embracedShield; } }
        public Weapon HandledWepon { get { return handledWeapon == null ? Weapon.UnarmedAttack : handledWeapon; } }
        public Backpack Backpack { get { return backpack; } }

        public TemporaryModifierCollection TempModifiers { get; private set; }

        public string RaceType
        {
            get
            {
                return this.GetType().Name.Clean();
            }
        }

        public God God { get; private set; }

        public List<Perception> Perceptions { get; private set; }

        public Character(string name,
                         int currentPf,
                         int maximumPf,
                         int hunger,
                         Stats stats,
                         Armor wornArmor,
                         Shield embracedShield,
                         Weapon handledWeapon,
                         Backpack backpack,
                         God god,
                         bool unblockable,
                         string symbol = "C",
                         Color color = new Color(),
                         string description = "A character of the game",
                         Coord position = new Coord())
            : base(name, symbol, color, false, false, unblockable, description, position)
        {
            this.hp = new int[] { currentPf, maximumPf };
            this.hunger = hunger;

            this.stats = stats;

            this.wornArmor = wornArmor;
            this.embracedShield = embracedShield;
            this.handledWeapon = handledWeapon;
            this.backpack = backpack;

            this.God = god;

            this.CharacterSheets = new List<ISheetViewer>();

            this.Perceptions = initPerceptions();
            this.TempModifiers = new TemporaryModifierCollection();
        }

        private List<Perception> initPerceptions()
        {
            var perceptions = this.GetType().GetCustomAttributes(typeof(HasPerception), false)
                                    .Select(hP => ((HasPerception)hP).Instantiate()).ToList();
            return perceptions;
        }

        public virtual void GainExperience(int xp)
        {
            
        }

        public void SufferDamage(Damage dmg)
        {
            var msg = new StringBuilder();
            var effectiveDmg = dmg - WornArmor.DamageReduction;
            var totalDmg = effectiveDmg.TotalDamage;
            hp[(int)HpType.Current] -= totalDmg;
            CharacterSheets.ForEach((sheet) => sheet.NotifyHp(Hp, MaxHp));
            msg.AppendFormat(   "Takes {0} damage{1} ", 
                                totalDmg,
                                totalDmg == 1 ? "" : "s");
            var dmgString = effectiveDmg.ToHorString();
            var show = dmgString.Length > 0;
            msg.AppendFormat(   "{0}{1}{2}", 
                                show ? "(" : "",
                                dmgString, 
                                show ? ")" : "");
            NotifyListeners(msg.ToString());
        }

        public void HealDamage(Damage dmg)
        {
            hp[(int)HpType.Current] += dmg.TotalDamage;
            CharacterSheets.ForEach((sheet) => sheet.NotifyHp(Hp, MaxHp));
        }

        public void DecreaseMaxHp(int deltaHp)
        {
            hp[(int)HpType.Max] -= deltaHp;
            CharacterSheets.ForEach((sheet) => sheet.NotifyHp(Hp, MaxHp));
        }

        public void IncreaseMaxHp(int deltaHp)
        {
            hp[(int)HpType.Max] += deltaHp;
            CharacterSheets.ForEach((sheet) => sheet.NotifyHp(Hp, MaxHp));
        }

        public void DecreaseStat(StatsType stat, int delta)
        {
            this.stats.DecreaseStat(stat, delta);
            CharacterSheets.ForEach((sheet) => sheet.NotifyStat(stat, stats[stat]));
        }

        public void IncreaseStat(StatsType stat, int delta)
        {
            this.stats.IncreaseStat(stat, delta);
            CharacterSheets.ForEach((sheet) => sheet.NotifyStat(stat, stats[stat]));
        }

        //public virtual bool Move(Direction dir, out bool acted)
        //{
        //    var candidateCoord = new Coord()
        //    {
        //        X = this.Position.X,
        //        Y = this.Position.Y
        //    };

        //    switch(dir)
        //    {
        //        case Direction.North:
        //            candidateCoord.Y = Math.Max(0, candidateCoord.Y - 1);
        //            break;
        //        case Direction.NorthEast:
        //            candidateCoord.Y = Math.Max(0, candidateCoord.Y - 1);
        //            candidateCoord.X = Math.Min(this.Map.Width - 1, candidateCoord.X + 1);
        //            break;
        //        case Direction.East:
        //            candidateCoord.X = Math.Min(this.Map.Width - 1, candidateCoord.X + 1);
        //            break;
        //        case Direction.SouthEast:
        //            candidateCoord.Y = Math.Min(this.Map.Height - 1, candidateCoord.Y + 1);
        //            candidateCoord.X = Math.Min(this.Map.Width - 1, candidateCoord.X + 1);
        //            break;
        //        case Direction.South:
        //            candidateCoord.Y = Math.Min(this.Map.Height - 1, candidateCoord.Y + 1);
        //            break;
        //        case Direction.SouthWest:
        //            candidateCoord.Y = Math.Min(this.Map.Height - 1, candidateCoord.Y + 1);
        //            candidateCoord.X = Math.Max(0, candidateCoord.X - 1);
        //            break;
        //        case Direction.West:
        //            candidateCoord.X = Math.Max(0, candidateCoord.X - 1);
        //            break;
        //        case Direction.NorthWest:
        //            candidateCoord.Y = Math.Max(0, candidateCoord.Y - 1);
        //            candidateCoord.X = Math.Max(0, candidateCoord.X - 1);
        //            break;
        //    }

        //    var moved = false;
        //    acted = false;

        //    if (this.Map.CanMoveTo(candidateCoord))
        //    {
        //        this.Map.MoveAtomTo(this, this.Position, candidateCoord);
        //        this.Position = candidateCoord;

        //        moved = true;
        //        acted = true;
        //    }
        //    else
        //    {
        //        acted = this.Map[candidateCoord].Interaction(this);
        //    }
            
        //    return moved;
        //}

        public void Attack(Character defenderCharachter)
        {
            var msg = new StringBuilder();

            msg.AppendFormat("Tries to hit {0} with {1}: ", defenderCharachter.Name, HandledWepon.Name);
            var tpc = Dice.Throws(20) + Stats[StatsType.Precision].ModifierOfStat() + HandledWepon.BonusOnTPC;
            var defenderCA = defenderCharachter.CA;
            if (tpc >= defenderCA)
            {
                var physicalDmg = new Damage();
                physicalDmg[DamageType.Physical] = this.stats[StatsType.Strength].ModifierOfStat();

                var actualDmg = this.HandledWepon.Damage + physicalDmg;

                msg.AppendFormat("HIT! ({0} vs. {1})", tpc, defenderCA);
                NotifyListeners(msg.ToString());
                defenderCharachter.SufferDamage(actualDmg);
            }
            else
            {
                msg.AppendFormat("MISSED... ({0} vs. {1})", tpc, defenderCA);
                NotifyListeners(msg.ToString());
            }
            

            msg.Clear();
            if (this.HandledWepon.HasSpecialAttack && this.HandledWepon.SpecialAttackActivated)
            {
                if (this.HandledWepon.Uses == -1 || this.HandledWepon.Uses > 0)
                {
                    var specialTpc = Dice.Throws(20) + Stats[StatsType.InnatePower].ModifierOfStat();
                    var specialCA = defenderCharachter.CASpecial;

                    msg.AppendFormat("Tries to use {0} special power: ", HandledWepon.Name);

                    if (specialTpc >= specialCA)
                    {
                        this.HandledWepon.SpecialAttack(this, defenderCharachter);
                        msg.AppendFormat("HIT! ({0} vs. {1})", specialTpc, specialCA);
                    }
                    else
                    {
                        msg.AppendFormat("MISSED... ({0} vs. {1})", specialTpc, specialCA);
                    }

                    NotifyListeners(msg.ToString());
                }
            }

            if(defenderCharachter.Dead)
            {
                NotifyListeners(String.Format("Kills {0}", defenderCharachter.Name));
                defenderCharachter.Die(this);
            }
        }

        public abstract void Die(Character killer);

        public bool PickUp()
        {
            NotifyListeners("*kneels to pick up something*");

            var candidate = this.Map.UnderneathAtom(this.Position);
            var type = candidate.GetType();
            if (candidate.IsPickable)
            {
                var itemType = typeof(Item);
                if (type == itemType || type.IsSubclassOf(itemType))
                {
                    var item = (Item)candidate;
                    PickUp(item);
                    NotifyListeners(String.Format("Picked up {0}[{1}]",
                                    item.Name,
                                    item.ItemTypeName));
                    return true;
                }
            }
            else
            {
                if(type == typeof(Floor))
                {
                    NotifyListeners("There's nothing here...");
                }
                else
                {
                    NotifyListeners("I can't pick up this...");
                }
            }

            return false;
        }

        private void PickUp(Item item)
        {
            this.Backpack.Add(item);
            this.Map.RemoveFromBuffer(item);
        }

        public void WearArmor(Item armor)
        {
            var canWear = armor.GetType().IsSubclassOf(typeof(Armor));
            if (canWear)
            {
                RemoveCurrent();
                NotifyListeners(String.Format("Put on {0}", armor.Name));
                wornArmor = (Armor)this.Backpack.Remove(armor);
                CharacterSheets.ForEach((sheet) => sheet.NotifyArmor(WornArmor));
            }
            else
            {
                NotifyListeners(String.Format("Can't put on {0}", armor.Name));
            }
        }

        public void RemoveArmor()
        {
            RemoveCurrent();
            wornArmor = null;
            CharacterSheets.ForEach((sheet) => sheet.NotifyArmor(WornArmor));
        }

        private void RemoveCurrent()
        {
            if (!WornArmor.IsSkin)
            {
                NotifyListeners(String.Format("Put off {0}", WornArmor.Name));
                this.Backpack.Add(wornArmor);
            }
        }

        public void EmbraceShield(Item shield)
        {
            var canEmbrace = shield.GetType().IsSubclassOf(typeof(Shield));
            if (canEmbrace)
            {
                DisembraceCurrent();
                embracedShield = (Shield)this.Backpack.Remove(shield);
                NotifyListeners(String.Format("Embrace {0}", shield.Name));
                CharacterSheets.ForEach((sheet) => sheet.NotifyShield(EmbracedShield));
                CharacterSheets.ForEach((sheet) => sheet.NotifyDefences(CA, CASpecial));
            }
            else
            {
                NotifyListeners(String.Format("Can't embrace {0}", shield.Name));
            }
        }

        public void DisembraceShield()
        {
            DisembraceCurrent();
            embracedShield = null;
            CharacterSheets.ForEach((sheet) => sheet.NotifyShield(EmbracedShield));
            CharacterSheets.ForEach((sheet) => sheet.NotifyDefences(CA, CASpecial));
        }

        private void DisembraceCurrent()
        {
            if (!EmbracedShield.IsSkin)
            {
                this.Backpack.Add(embracedShield);
                NotifyListeners(String.Format("Put away {0}", EmbracedShield.Name));
            }
        }

        public void HandleWeapon(Item weapon)
        {
            var canHandle = weapon.GetType().IsSubclassOf(typeof(Weapon));
            
            if (canHandle)
            {
                UnhandleCurrent();
                handledWeapon = (Weapon)this.Backpack.Remove(weapon);
                NotifyListeners(String.Format("Handles {0}", HandledWepon.Name));
                CharacterSheets.ForEach((sheet) => sheet.NotifyWeapon(HandledWepon));
            }
            else
            {
                NotifyListeners(String.Format("Can't handle {0}", weapon.Name));
            }
        }

        public void UnhandleWeapon()
        {
            UnhandleCurrent();
            handledWeapon = null;
            CharacterSheets.ForEach((sheet) => sheet.NotifyWeapon(HandledWepon));
        }

        private void UnhandleCurrent()
        {
            if (!HandledWepon.IsUnarmed)
            {
                NotifyListeners(String.Format("Sheat {0}", HandledWepon.Name));
                this.Backpack.Add(handledWeapon);
            }
        }

        public void Pray(out bool acted)
        {
            acted = true;

            if(God != null)
            {
                NotifyListeners(String.Format("*kneels to pray*"));
                TemporaryModifier mod;
                God.PrayResult res;
                if ((res = God.HearPray(out mod)) != God.PrayResult.None)
                {
                    var msg = new StringBuilder(String.Format("{0} hears you", God.Name));

                    switch (res)
                    {
                        case God.PrayResult.Bad:
                            msg.Append("but he's very mad at you...");
                            break;
                        case God.PrayResult.Good:
                            msg.Append("and he decided to grant you his favours");
                            break;
                        case God.PrayResult.VeryGood:
                            msg.Append("and blesses you as his holy champion");
                            break;
                    }

                    NotifyListeners(msg.ToString());
                    //TempModifiers.AddTemporaryModifier(mod);
                    RegisterTemporaryMod(mod);
                }
                else
                {
                    NotifyListeners(String.Format("{0} is deaf to your requests", God.Name));
                }
            }
            else
            {
                NotifyListeners("*Tries to pray*");
                NotifyListeners("But no god will come to help you");
                acted = false;
            }
        }

        public virtual void EffectOfTurn()
        {
            var expiredMods = 0;
            if((TempModifiers.PassedTurn()) > 0)
            {
                NotifyListeners(String.Format("{0} modifier{1} {2} expired",
                                              expiredMods,
                                              expiredMods == 1 ? "" : "s",
                                              expiredMods == 1 ? "has" : "have"));
            }
        }

        protected bool SomethingBlockView(Coord other)
        {
            var line = new Line(Position, other);
            foreach (Coord pt in line)
            {
                if (Map[pt].BlockVision)
                {
                    return true;
                }
            }

            return false;
        }

        public virtual void RegisterTemporaryMod(TemporaryModifier mod)
        {
            TempModifiers.AddTemporaryModifier(mod);
            NotifyListeners(String.Format("Received mod {0} [x{1} Turns]", 
                            mod.ToString(),
                            mod.TimeToLive));
        }

        public virtual void RegisterSheet(ISheetViewer sheet)
        {
            CharacterSheets.Add(sheet);
        }
    }
}
