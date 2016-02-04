using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    [AttributeUsage(AttributeTargets.All)]
    public class WeaponDescription : Attribute
    {
        public string Description { get; private set; }

        public WeaponDescription(string description)
        {
            Description = description;
        }
    }
}
