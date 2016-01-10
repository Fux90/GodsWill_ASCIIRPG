using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Core
{
    static class Dice
    {
        static Random rnd;
        public delegate int SumMethod(int[] partialResults);

        public static int Throws(int nFaces, int nDice = 1, SumMethod sumMethod = null)
        {
            if(rnd == null)
            {
                rnd = new Random();
            }

            var partial = new int[nDice];
            for (int i = 0; i < nDice; i++)
            {
                partial[i] = rnd.Next(1, nFaces);
            }

            return sumMethod == null
                ? partial.Sum()
                : sumMethod(partial);
        }
    }
}
