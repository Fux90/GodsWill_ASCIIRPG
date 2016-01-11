using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Core
{
    public class Dice
    {
        static Random rnd;
        public delegate int CountingMethod(int[] partialResults);

        public int NumFaces { get; private set; }
        public int NumDice { get; private set; }
        public int Min { get { return NumDice; } }
        public int Max { get { return NumFaces * NumDice; } }

        public Dice(int nFaces = 1, int nDice = 1)
        {
            NumFaces = nFaces;
            NumDice = nDice;
        }

        public static int Throws(Dice dice, int nThrows = 1, CountingMethod countingMethod = null)
        {
            return Throws(dice.NumFaces, dice.NumDice, nThrows, countingMethod);
        }

        public static int Throws(int nFaces, int nDice = 1, int nThrows = 1, CountingMethod countingMethod = null)
        {
            if(rnd == null)
            {
                rnd = new Random();
            }

            var totThrows = nThrows * nDice;

            if(totThrows == 0)
            {
                return 0;
            }

            var partial = new int[totThrows];
            for (int i = 0; i < totThrows; i++)
            {
                partial[i] = rnd.Next(1, nFaces);
            }

            return countingMethod == null
                ? partial.Sum()
                : countingMethod(partial);
        }
    }
}
