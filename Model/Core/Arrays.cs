using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Core
{
    public class BidimensionalArray<T>
    {
        public delegate T Instantiator();

        T[,] content;
        int height;
        int width;

        public int Height { get { return height; } }
        public int Width { get { return width; } }
        public int Rows { get { return height; } }
        public int Cols { get { return width; } }

        public BidimensionalArray(int rows, int cols)
        {
            content = new T[rows, cols];
            height = rows;
            width = cols;
        }

        public BidimensionalArray(int rows, int cols, T defaultValue)
        {
            content = new T[rows, cols];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    content[r, c] = defaultValue;
                }
            }
        }

        public BidimensionalArray(int rows, int cols, Instantiator instantiator)
        {
            content = new T[rows, cols];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    content[r, c] = instantiator();
                }
            }
        }

        public T this[int row, int col]
        {
            get
            {
                return content[row, col];
            }

            set
            {
                content[row, col] = value;
            }
        }

        public T this[Coord coord]
        {
            get
            {
                return content[coord.Y, coord.X];
            }

            set
            {
                content[coord.Y, coord.X] = value;
            }
        }

        public void ForEach(Action<T> action)
        {
            for (int r = 0; r < this.Rows; r++)
            {
                for (int c = 0; c < this.Cols; c++)
                {
                    action((T)content[r, c]);
                }
            }
        }
    }
}
