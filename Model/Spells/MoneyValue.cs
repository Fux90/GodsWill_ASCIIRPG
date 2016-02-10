using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Spells
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MoneyValue : Attribute
    {
        public int Value { get; private set; }

        public MoneyValue(int value)
        {
            Value = value;        
        }
    }
}
