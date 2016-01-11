using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.AICharacters
{
    public class Orc : AICharacter
    {
        public Orc( string name,
                    int currentPf)
            : base(name,
                  currentPf,
                  currentPf,
                  10,
                  new SimpleAI(this))
        {

        }
    }
}
