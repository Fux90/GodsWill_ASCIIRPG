using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Core
{
    public enum ThrowModifier
    {
        Normal,
        Maximized,
        Minimized,
        Averaged,
    }

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

        public static int Throws(Dice dice, 
                                 int nThrows = 1, 
                                 CountingMethod countingMethod = null, 
                                 ThrowModifier mod = ThrowModifier.Normal)
        {
            return Throws(dice.NumFaces, dice.NumDice, nThrows, countingMethod, mod);
        }

        public static int Throws(   int nFaces, 
                                    int nDice, 
                                    int nThrows, 
                                    CountingMethod countingMethod)
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
            var upperBound = nFaces + 1;

            for (int i = 0; i < totThrows; i++)
            {
                partial[i] = rnd.Next(1, upperBound);
            }

            return countingMethod == null
                ? partial.Sum()
                : countingMethod(partial);
        }

        public static int Throws(int nFaces, int nDice = 1, int nThrows = 1, CountingMethod countingMethod = null, ThrowModifier mod = ThrowModifier.Normal)
        {
            switch(mod)
            {
                case ThrowModifier.Minimized:
                    return nDice * nThrows;
                case ThrowModifier.Maximized:
                    return nFaces * nDice * nThrows;
                case ThrowModifier.Averaged:
                    return (int)Math.Floor((((nDice * nThrows) * (1 + nFaces)) / 2.0) * nThrows);
                case ThrowModifier.Normal:
                default:
                    return Throws(nFaces, nDice, nThrows, countingMethod);
            }
        }
    }
}
