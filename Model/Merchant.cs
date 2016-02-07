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
    [Serializable]
    public class Merchant : Atom, 
        IGoldDealer, IViewable<IMerchantViewer>, ISerializable, IMerchant
    {
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
        public Backpack Backpack { get; private set; }
        public MerchantInclination Inclination { get; private set; }

        public Merchant(string name,
                        string symbol,
                        Color color,
                        Backpack backpack,
                        string description = "A Merchant",
                        Coord position = new Coord())
            : base(name, symbol, color, true, false, description, position)
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

        public void RegisterView(IMerchantViewer view)
        {
            merchantView = view;
        }

        public bool GiveAwayGold(int amount, out Gold gold)
        {
            gold = new Gold(amount);
            // A merchant has always cash!!
            return true;
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
                BeginTransaction();
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

        public void BeginTransaction()
        {
            merchantView.BringUpAndFocus();
        }
    }
}
