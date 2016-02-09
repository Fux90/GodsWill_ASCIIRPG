using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    public static partial class Triggerables
    {
        public static readonly ITriggerable None = null;
    }

    public interface ITriggerable
    {
        Atom TriggeringSubject { get; }
        bool ImmediateTrigger { get; }
        void Trigger();
        void RegisterTriggeringSubject(Atom triggeringSubject);
    }

    public interface ITriggerActor
    {
        ITriggerable CurrentTriggerable { get; }
        bool TriggerCurrent();
        void RegisterTriggerable(ITriggerable triggerable);
        void UnregisterTriggerable();
    }
}
