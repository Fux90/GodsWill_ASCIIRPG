using GodsWill_ASCIIRPG.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.View
{
    public interface ISpeelbookViewer : IViewer
    {
        void NotifyAdd(Spell[] spells);
    }
}
