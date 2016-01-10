﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Core
{
    public class BidimensionalArray<T>
    {
        T[,] content;
        int height;
        int width;

        public int Height { get { return height; } }
        public int Width { get { return width; } }
        public int Rows { get { return width; } }
        public int Cols { get { return height; } }

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

        public T this[int row, int col]
        {
            get
            {
                return content[col, row];
            }

            set
            {
                content[col, row] = value;
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
    }
}