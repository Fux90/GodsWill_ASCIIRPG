using GodsWill_ASCIIRPG.Model;
using GodsWill_ASCIIRPG.Model.Core;
using GodsWill_ASCIIRPG.Model.Items;
using GodsWill_ASCIIRPG.View;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GodsWill_ASCIIRPG
{
    [Serializable]
    [StraightSightNeededForPerception]
    public abstract class Character : MoveableAtom, 
        IFighter, IDamageable, IBlockable, IGoldDealer, ISerializable,
        IViewable<ISheetViewer>
    {
        #region SERIALIZABLE_CONST_NAMES
        const string hpSerializableName = "hp";
        const string hungerSerializableName = "hunger";
        const string wornArmorSerializableName = "wornArmor";
        const string embracedShieldSerializableName = "embraceShield";
        const string handledWeaponSerializableName = "handledWeapon";
        const string backpackSerializableName = "backpack";
        const string statsSerializableName = "stats";
        const string myGoldSerializableName = "gold";
        const string tempModifiersSerializableName = "tempMods";
        const string godSerializableName = "god";
        const string perceptionsSerializableName = "perceptions";
        const string blockedTurnsSerializableName = "blockedTurns";
        const string alliedToSerializableName = "alliedTo";
        #endregion

        protected enum HpType
        {
            Current,
            Max
        }

        public Allied AlliedTo
        {
            get; protected set;
        }

        int[] hp;
        int hunger;
        Armor wornArmor;
        Shield embracedShield;
        Weapon handledWeapon;
        Backpack backpack;
        //Spellbook spellbook;
        Stats stats;

        public int MyGold { get; protected set; }

        private List<ISheetViewer> characterSheets;
        protected List<ISheetViewer> CharacterSheets{ get { return characterSheets; }}

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
        //public Spellbook Spellbook{ get { return spellbook; } }

        private TemporaryModifierCollection tempModifiers;
        public TemporaryModifierCollection TempModifiers
        {
            get
            {
                return tempModifiers;
            }

            private set
            {
                tempModifiers = value;
            }
        }

        public string RaceType
        {
            get
            {
                return this.GetType().Name.Clean();
            }
        }

        public God God { get; private set; }

        public List<Perception> perceptions;
        public List<Perception> Perceptions
        {
            get { return perceptions; }
            private set { perceptions = value; }
        }

        public int BlockedTurns { get; private set; }

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

            Init();
        }

        public Character(   SerializationInfo info,
                            StreamingContext context)
            : base(info, context)
        {
            hp = (int[])info.GetValue(hpSerializableName, typeof(int[]));
            hunger = (int)info.GetValue(hungerSerializableName, typeof(int));
            wornArmor = (Armor)info.GetValue(wornArmorSerializableName, typeof(Armor));
            embracedShield = (Shield)info.GetValue(embracedShieldSerializableName, typeof(Shield));
            handledWeapon = (Weapon)info.GetValue(handledWeaponSerializableName, typeof(Weapon));
            backpack = (Backpack)info.GetValue(backpackSerializableName, typeof(Backpack));
            stats = (Stats)info.GetValue(statsSerializableName, typeof(Stats));
            MyGold = (int)info.GetValue(myGoldSerializableName, typeof(int));
            TempModifiers = (TemporaryModifierCollection)info.GetValue(tempModifiersSerializableName, typeof(TemporaryModifierCollection));
            God = (God)info.GetValue(godSerializableName, typeof(God));
            Perceptions = (List<Perception>)info.GetValue(perceptionsSerializableName, typeof(List<Perception>));
            BlockedTurns = (int)info.GetValue(blockedTurnsSerializableName, typeof(int));
            AlliedTo = (Allied)info.GetValue(alliedToSerializableName, typeof(Allied));
            Init();
        }

        private void Init()
        {
            CreateIfNull(ref this.characterSheets, () => new List<ISheetViewer>());
            CreateIfNull(ref this.perceptions, () => initPerceptions());
            CreateIfNull(ref this.tempModifiers, () => new TemporaryModifierCollection());
        }

        private delegate T Create<T>();
        private void CreateIfNull<T>(ref T obj, Create<T> creation)
        {
            if(obj == null)
            {
                obj = creation();
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(hpSerializableName, hp, typeof(int[]));
            info.AddValue(hungerSerializableName, hunger, typeof(int));
            info.AddValue(wornArmorSerializableName, wornArmor, typeof(Armor));
            info.AddValue(embracedShieldSerializableName, embracedShield, typeof(Shield));
            info.AddValue(handledWeaponSerializableName, handledWeapon, typeof(Weapon));
            info.AddValue(backpackSerializableName, backpack, typeof(Backpack));
            info.AddValue(statsSerializableName, stats, typeof(Stats));
            info.AddValue(myGoldSerializableName, MyGold, typeof(int));
            info.AddValue(tempModifiersSerializableName, TempModifiers, typeof(TemporaryModifierCollection));
            info.AddValue(godSerializableName, God, typeof(God));
            info.AddValue(perceptionsSerializableName, Perceptions, typeof(List<Perception>));
            info.AddValue(blockedTurnsSerializableName, BlockedTurns, typeof(int));
            info.AddValue(alliedToSerializableName, AlliedTo, typeof(Allied));
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

        public virtual void PickUpGold(Gold gold)
        {
            this.MyGold += gold.Amount;
            this.Map.Remove(gold);
            this.NotifyListeners(String.Format("Gained {0} gold pieces", gold.Amount));
        }

        public virtual void UseItem(Item item)
        {
            item.ActiveUse(this);
        }

        public bool GiveAwayGold(int amount, out Gold gold)
        {
            gold = new Gold(Math.Min(MyGold, amount));
            if (MyGold >= amount)
            {
                MyGold -= amount;
                this.NotifyListeners(String.Format("Gived away {0} gold pieces", gold.Amount));
                return true;
            }
            this.NotifyListeners("Not enough gold to fullfil the request...");
            return false;
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

        public void Attack(IDamageable defenderCharachter)
        {
            var msg = new StringBuilder();

            msg.AppendFormat("Tries to hit {0} with {1}: ", ((Atom)defenderCharachter).Name, HandledWepon.Name);
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
                NotifyListeners(String.Format("Kills {0}", ((Atom)defenderCharachter).Name));
                defenderCharachter.Die(this);
            }
        }

        public abstract void Die(IFighter killer);

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

        public void ActivateSpecialAttack()
        {
            if(HandledWepon.HasSpecialAttack)
            {
                if (HandledWepon.SpecialAttackActivated)
                {
                    HandledWepon.DectivateSpecialAttack();
                    NotifyListeners(String.Format("Dectivated {0}'s special power", HandledWepon.Name));
                }
                else
                {
                    HandledWepon.ActivateSpecialAttack();
                    NotifyListeners(String.Format("Activated {0}'s special power", HandledWepon.Name));
                }
                CharacterSheets.ForEach((sheet) => sheet.NotifyWeapon(HandledWepon));
            }
            else
            {
                NotifyListeners(String.Format("{0} has no special attacks", HandledWepon.Name));
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
            ConsumeModifiers();
        }

        private void ConsumeModifiers()
        {
            var expiredMods = TempModifiers.PassedTurn();
            if (expiredMods > 0)
            {
                NotifyListeners(String.Format("{0} modifier{1} {2} expired",
                                              expiredMods,
                                              expiredMods == 1 ? "" : "s",
                                              expiredMods == 1 ? "has" : "have"));
                NotifyAll();
            }
        }

        protected virtual void NotifyAll()
        {
            CharacterSheets.ForEach((sheet) => sheet.NotifyName(this.Name));
            //CharacterSheets.ForEach((sheet) => sheet.NotifyLevel(this.CurrentLevel, this.God));
            //CharacterSheets.ForEach((sheet) => sheet.NotifyXp(this.XP, this.NextXP));
            CharacterSheets.ForEach((sheet) => sheet.NotifyGold(this.MyGold));
            CharacterSheets.ForEach((sheet) => sheet.NotifyHp(this.Hp, this.MaxHp));
            CharacterSheets.ForEach((sheet) => sheet.NotifyHunger(this.Hunger));
            CharacterSheets.ForEach((sheet) => sheet.NotifyDefences(this.CA, this.CASpecial));
            CharacterSheets.ForEach((sheet) => sheet.NotifyArmor(this.WornArmor));
            CharacterSheets.ForEach((sheet) => sheet.NotifyShield(this.EmbracedShield));
            CharacterSheets.ForEach((sheet) => sheet.NotifyWeapon(this.HandledWepon));
            CharacterSheets.ForEach((sheet) => {
                foreach (var stat in Stats.AllStats)
                {
                    sheet.NotifyStat(stat, this.Stats[stat]);
                }
            });
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

        public virtual void RegisterView(ISheetViewer sheet)
        {
            CharacterSheets.Add(sheet);
        }

        public void BlockForTurns(int numTurns)
        {
            BlockedTurns = numTurns;
        }

        public virtual void SkipTurn()
        {
            BlockedTurns--;
        }
    }
}
