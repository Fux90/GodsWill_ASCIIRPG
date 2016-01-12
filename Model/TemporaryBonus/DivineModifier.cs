using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.TemporaryBonus
{
    [ApplyableTo(ModFor.CA)]
    [ApplyableTo(ModFor.Stats)]
    public class DivineModifier<T> : TemporaryModifier<T>
    {
        public DivineModifier(int ttl, ModFor applyTo, T bonus)
            : base(ttl, applyTo, bonus)
        {

        }
    }
}
