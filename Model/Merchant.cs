using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GodsWill_ASCIIRPG.Model.Items;
using System.Runtime.Serialization;
using System.Drawing;
using GodsWill_ASCIIRPG.View;
using GodsWill_ASCIIRPG.Model.Core;

namespace GodsWill_ASCIIRPG.Model
{
    public class MerchantBuilder
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public Color Color { get; set; }
        public Backpack Backpack { get; set; }
        public string Description { get; set; }
        public Coord Position { get; set; }

        public MerchantBuilder()
        {
            Name = RandomName();
            Symbol = Merchant.defaultSymbol;
            Color = Merchant.defaultColor;
            Backpack = new Backpack();
            Description = "A Merchant";
            Position = new Coord();
        }

        private string RandomName()
        {
            return "Dalek, the Innkeeper";
        }

        public bool AddItem(Item item)
        {
            if(Backpack == null)
            {
                return false;
            }

            Backpack.Add(item);
            return true;
        }

        public Merchant Build()
        {
            return new Merchant(Name,
                                Symbol,
                                Color,
                                Backpack,
                                Description,
                                Position);
        }
    }

    [Serializable]
    public class Merchant : TwoLevelNotifyableAtom, 
        IGoldDealer, IMerchant, 
        IViewable<IMerchantViewer>,
        ISerializable
    {
        public const string defaultSymbol = "M";
        public static readonly Color defaultColor = Color.Purple;

        public enum MerchantInclination
        {
            Friendly,
            Normal,
            Unfriendly
        }

        public class DealingMod
        {
            public float Purchase { get; private set; }
            public float Sell { get; private set; }

            public DealingMod(float purchase, float sell)
            {
                Purchase = purchase;
                Sell = sell;
            }

            public DealingMod()
                : this(1.0f, 1.0f)
            {

            }
        }

        public class InclinationRange
        {
            public int UnfriendlyThr { get; private set; }
            public int FriendlyThr { get; private set; }

            public InclinationRange(int unfriendlyThr, int friendlyThr)
            {
                this.UnfriendlyThr = unfriendlyThr;
                this.FriendlyThr = friendlyThr;
            }

            public InclinationRange()
                : this(10, 80)
            {

            }
        }

        private const string backpackSerializableName = "backpack";

        private readonly Dictionary<MerchantInclination, DealingMod>
            GoldModifiersByInclination = new Dictionary<MerchantInclination, DealingMod>()
            {
                {MerchantInclination.Friendly, new DealingMod(purchase: 1.10f, sell: 0.80f)},
                {MerchantInclination.Normal, new DealingMod()},
                {MerchantInclination.Unfriendly, new DealingMod(purchase: 0.95f, sell: 1.10f)},
            };

        private readonly Dictionary<Pg.Level, InclinationRange>
            ThresholdInclinationByLevel = new Dictionary<Pg.Level, InclinationRange>()
            {
                {Pg.Level.Novice, new InclinationRange(20, 95) },
                {Pg.Level.Cleric, new InclinationRange(15, 90) },
                {Pg.Level.Master, new InclinationRange(10, 90) },
                {Pg.Level.GrandMaster, new InclinationRange(5, 85) },
            };

        private IMerchantViewer merchantView;
        private List<IAtomListener> secondaryListeners;
        private List<IAtomListener> SecondaryListeners
        {
            get
            {
                if(secondaryListeners == null)
                {
                    secondaryListeners = new List<IAtomListener>();
                }

                return secondaryListeners;
            }
        }

        public Backpack Backpack { get; private set; }
        public MerchantInclination Inclination { get; private set; }

        public Merchant(string name,
                        string symbol,
                        Color color,
                        Backpack backpack,
                        string description,
                        Coord position)
            : base(name, symbol, color, false, true, description, position)
        {
            Backpack = backpack;
        }

        public Merchant(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Backpack = (Backpack)info.GetValue(backpackSerializableName, typeof(Backpack));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(backpackSerializableName, Backpack, typeof(Backpack));
        }

        public bool GiveAwayGold(int amount, out Gold gold)
        {
            gold = new Gold(amount);
            // A merchant has always cash!!
            return true;
        }

        public void ProposeSell(Item item)
        {
            if (item.IsSellable)
            {
                var cost = (int)Math.Round(GoldModifiersByInclination[Inclination].Sell * item.Cost);
                NotifyListeners(String.Format("{0}: \"I can give you this for {1} gold pieces\"", this.Name, cost));
            }
            else
            {
                NotifyListeners("I can't sell this to you...");
            }
        }

        public void ProposePurchase(Item item)
        {
            if (item.IsSellable)
            {
                var cost = (int)Math.Round(GoldModifiersByInclination[Inclination].Purchase * item.Cost);
                NotifyListeners(String.Format("{0}: \"For this I can give you {1} gold pieces\"", this.Name, cost));
            }
            else
            {
                NotifyListeners("Mmm... I can't buy this from you");
            }
        }

        public void NoProposal()
        {
            NotifyListeners("");
        }

        public bool Sell(Item item, Pg to)
        {
            if (item.IsSellable)
            {
                var cost = (int)Math.Round(GoldModifiersByInclination[Inclination].Sell * item.Cost);

                Gold gold;
                if (to.GiveAwayGold(cost, out gold))
                {
                    this.PickUpGold(gold);

                    var itemType = item.GetType();
                    if (!typeof(ItemStack).IsAssignableFrom(itemType)
                        || item.SellableOnlyAsFullStack)
                    {
                        this.Backpack.Remove(item);
                        to.Backpack.Add(item);
                    }
                    else
                    {
                        var firstItem = ((ItemStack)item)[0];
                        this.Backpack.Remove(firstItem);
                        to.Backpack.Add(firstItem);
                    }
                    

                    merchantView.NotifyBuyerGold(to.MyGold);
                    NotifyExcange();

                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        public bool Purchase(Item item, Pg from)
        {
            if (item.IsSellable)
            {
                var cost = (int)Math.Round(GoldModifiersByInclination[Inclination].Purchase * item.Cost);

                Gold gold;
                this.GiveAwayGold(cost, out gold);

                from.PickUpGold(gold);

                var itemType = item.GetType();
                if (!typeof(ItemStack).IsAssignableFrom(itemType)
                    || item.SellableOnlyAsFullStack)
                {
                    from.Backpack.Remove(item);
                    this.Backpack.Add(item);
                }
                else
                {
                    var firstItem = ((ItemStack)item)[0];
                    from.Backpack.Remove(firstItem);
                    this.Backpack.Add(firstItem);
                }

                merchantView.NotifyBuyerGold(from.MyGold);
                NotifyExcange();

                return true;
            }

            return false;
        }

        private void NotifyExcange()
        {
            merchantView.NotifyExcange();
        }

        public override bool Interaction(Atom interactor)
        {
            if(typeof(Pg).IsAssignableFrom(interactor.GetType()))
            {
                var pgInteractor = (Pg)interactor;

                // 1) Evaluate inclination
                EvaluateInclination(pgInteractor);

                // 2) Bring up view
                // 3) Focus on merchant controller
                BeginTransaction(pgInteractor);
            }

            return false;
        }

        private void EvaluateInclination(Pg interactor)
        {
            var thr = this.ThresholdInclinationByLevel[interactor.CurrentLevel];

            var res = Dice.Throws(new Dice(100));

            if (res <= thr.UnfriendlyThr)
            {
                Inclination = MerchantInclination.Unfriendly;
            }
            else if (res <= thr.UnfriendlyThr)
            {
                Inclination = MerchantInclination.Friendly;
            }
            else
            {
                Inclination = MerchantInclination.Normal;
            }
        }

        public void PickUpGold(Gold gold)
        {
            // Nothing...
        }

        public void BeginTransaction(Pg interactor)
        {
            merchantView.NotifyMerchantName(this.Name);
            merchantView.BringUpAndFocus(interactor);
        }

        public virtual void BadSellSpeech()
        {
            NotifyListeners("Mmm... That's not enough gold");
        }

        public virtual void GoodSellSpeech()
        {
            NotifyListeners("It's a deal");
        }

        public virtual void GoodPurchaseSpeech()
        {
            NotifyListeners("Here you are");
        }

        public virtual void BadPurchaseSpeech()
        {
            NotifyListeners("I told you I can't buy that!");
        }

        public void GreetingsMessage()
        {
            NotifySecondaryViewers("Greetings");
        }

        public void FarewellMessage()
        {
            NotifySecondaryViewers("Farewell");
        }

        public void RegisterView(IMerchantViewer view)
        {
            merchantView = view;
        }

        public void SaysWhatSold(Item item, Pg controlledPg)
        {
            controlledPg.NotifyListeners(String.Format("Bought {0}", item.Name));
        }

        public void SaysWhatPurchased(Item item, Pg controlledPg)
        {
            controlledPg.NotifyListeners(String.Format("Sold {0}", item.Name));
        }
    }
}
