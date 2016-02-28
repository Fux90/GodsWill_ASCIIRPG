using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Items
{
    public interface IIdentifiable
    {
        bool IsIdentified { get; }
        void Identify(Atom identifier, int throwToIdentify);
    }
}
