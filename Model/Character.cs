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
    public abstract class Character : Atom
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

        public int Hp { get { return hp[(int)HpType.Current]; } }
        public int MaxHp { get { return hp[(int)HpType.Max]; } }
        public int Hunger { get { return hunger; } }
        public bool Dead { get { return hp[(int)HpType.Current] <= 0 || hunger <= 0 || stats.OneIsLessThanZero(); } }
        public Stats Stats { get { return stats; } }

        public int CA //Physical attacks
        {
            get
            {
                return 10 + EmbracedShield.BonusCA + Stats[StatsType.Agility];
            }
        }

        public int CASpecial //Mental attacks
        {
            get
            {
                return 10 + EmbracedShield.BonusSpecialCA + Stats[StatsType.Mental];
            }
        }

        public Armor WornArmor { get { return wornArmor == null ? Armor.Skin : wornArmor; } }
        public Shield EmbracedShield { get { return embracedShield == null ? Shield.NoShield : embracedShield; } }
        public Weapon HandledWepon { get { return handledWeapon == null ? Weapon.UnarmedAttack : handledWeapon; } }
        public Backpack Backpack { get { return backpack; } }

        public Character(string name,
                         int currentPf,
                         int maximumPf,
                         int hunger,
                         Stats stats,
                         Armor wornArmor,
                         Shield embracedShield,
                         Weapon handledWeapon,
                         Backpack backpack,
                         string symbol = "C",
                         Color color = new Color(),
                         string description = "A character of the game",
                         Coord position = new Coord())
            : base(name, symbol, color, false, description, position)
        {
            this.hp = new int[] { currentPf, maximumPf };
            this.hunger = hunger;

            this.stats = stats;

            this.wornArmor = wornArmor;
            this.embracedShield = embracedShield;
            this.handledWeapon = handledWeapon;
            this.backpack = backpack;

            this.CharacterSheets = new List<ISheetViewer>();
        }

        public virtual void GainExperience(int xp)
        {
            
        }

        public void SufferDamage(Damage dmg)
        {
            var effectiveDmg = dmg - WornArmor.DamageReduction;
            hp[(int)HpType.Current] -= effectiveDmg.TotalDamage;
            CharacterSheets.ForEach((sheet) => sheet.NotifyHp(Hp, MaxHp));
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

        public bool Move(Direction dir)
        {
            var candidateCoord = new Coord()
            {
                X = this.Position.X,
                Y = this.Position.Y
            };

            switch(dir)
            {
                case Direction.North:
                    candidateCoord.Y = Math.Max(0, candidateCoord.Y - 1);
                    break;
                case Direction.NorthEast:
                    candidateCoord.Y = Math.Max(0, candidateCoord.Y - 1);
                    candidateCoord.X = Math.Min(this.Map.Width - 1, candidateCoord.X + 1);
                    break;
                case Direction.East:
                    candidateCoord.X = Math.Min(this.Map.Width - 1, candidateCoord.X + 1);
                    break;
                case Direction.SouthEast:
                    candidateCoord.Y = Math.Min(this.Map.Height - 1, candidateCoord.Y + 1);
                    candidateCoord.X = Math.Min(this.Map.Width - 1, candidateCoord.X + 1);
                    break;
                case Direction.South:
                    candidateCoord.Y = Math.Min(this.Map.Height - 1, candidateCoord.Y + 1);
                    break;
                case Direction.SouthWest:
                    candidateCoord.Y = Math.Min(this.Map.Height - 1, candidateCoord.Y + 1);
                    candidateCoord.X = Math.Max(0, candidateCoord.X - 1);
                    break;
                case Direction.West:
                    candidateCoord.X = Math.Max(0, candidateCoord.X - 1);
                    break;
                case Direction.NorthWest:
                    candidateCoord.Y = Math.Max(0, candidateCoord.Y - 1);
                    candidateCoord.X = Math.Max(0, candidateCoord.X - 1);
                    break;
            }

            var moved = false;

            if (this.Map.CanMoveTo(candidateCoord))
            {
                this.Map.MoveAtomTo(this, this.Position, candidateCoord);
                this.Position = candidateCoord;

                moved = true;
            }
            else
            {
                this.Map[candidateCoord].Interaction(this);
            }
            
            return moved;
        }

        public void Attack(Character otherCharachter)
        {
            var tpc = Dice.Throws(20) + this.stats.ModifierOfStat(StatsType.Precision) + HandledWepon.BonusOnTPC;
            if(tpc >= otherCharachter.CA)
            {
                var physicalDmg = new Damage();
                physicalDmg[DamageType.Physical] = this.stats[StatsType.Strength];

                var actualDmg = this.HandledWepon.Damage + physicalDmg;

                otherCharachter.SufferDamage(actualDmg);
            }
            var specialTpc = Dice.Throws(20) + this.stats.ModifierOfStat(StatsType.InnatePower);
            if(specialTpc >= otherCharachter.CASpecial)
            {
                this.HandledWepon.SpecialAttack(this, otherCharachter);
            }

            if(otherCharachter.Dead)
            {
                otherCharachter.Die(this);
            }
        }

        public abstract void Die(Character killer);

        public void PickUp()
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
        }

        private void PickUp(Item item)
        {
            this.Backpack.Add(item);
            this.Map.RemoveFromBuffer(item);
        }

        public void WearArmor(Armor armor)
        {
            if(!WornArmor.IsSkin)
            {
                this.Backpack.Add(wornArmor);
            }
            wornArmor = (Armor)this.Backpack.Remove(armor);
            CharacterSheets.ForEach((sheet) => sheet.NotifyArmor(WornArmor));
        }

        public void RemoveArmor()
        {
            if (!WornArmor.IsSkin)
            {
                this.Backpack.Add(wornArmor);
            }
            wornArmor = null;
            CharacterSheets.ForEach((sheet) => sheet.NotifyArmor(WornArmor));
        }

        public void EmbraceShield(Shield shield)
        {
            if (!WornArmor.IsSkin)
            {
                this.Backpack.Add(embracedShield);
            }
            embracedShield = (Shield)this.Backpack.Remove(shield);
            CharacterSheets.ForEach((sheet) => sheet.NotifyShield(EmbracedShield));
        }

        public void DisembraceShield()
        {
            if (!WornArmor.IsSkin)
            {
                this.Backpack.Add(embracedShield);
            }
            embracedShield = null;
            CharacterSheets.ForEach((sheet) => sheet.NotifyShield(EmbracedShield));
        }

        public void HandleWeapon(Weapon weapon)
        {
            if (!HandledWepon.IsUnarmed)
            {
                this.Backpack.Add(handledWeapon);
            }
            handledWeapon = (Weapon)this.Backpack.Remove(weapon);
            CharacterSheets.ForEach((sheet) => sheet.NotifyWeapon(HandledWepon));
        }

        public void UnhandleWeapon(Weapon weapon)
        {
            if (!HandledWepon.IsUnarmed)
            {
                this.Backpack.Add(handledWeapon);
            }
            handledWeapon = null;
            CharacterSheets.ForEach((sheet) => sheet.NotifyWeapon(HandledWepon));
        }

        public virtual void RegisterSheet(ISheetViewer sheet)
        {
            CharacterSheets.Add(sheet);
        }
    }
}
