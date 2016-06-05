using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.AICharacters
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RandomEnemyGenerable : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class Enemy : Attribute
    {
    }
}
