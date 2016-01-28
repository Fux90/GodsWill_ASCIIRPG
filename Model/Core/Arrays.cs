using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Core
{
    [Serializable]
    public class BidimensionalArray<T> : ISerializable
    {
        #region DELEGATES
        public delegate T Instantiator();
        public delegate T CoordInstantiator(Coord pos);
        #endregion

        #region SERIALIZATION_CONST_NAMES
        private const string contentSerializableName = "name";
        private const string heightSerializableName = "height";
        private const string widthSerializableName = "width";
        #endregion

        #region DATA_MEMBERS
        T[,] content;
        int height;
        int width;
        #endregion

        #region PROPERTIES
        public int Height { get { return height; } }
        public int Width { get { return width; } }
        public int Rows { get { return height; } }
        public int Cols { get { return width; } }
        #endregion

        #region CONSTRUCTORS
        public BidimensionalArray(int rows, int cols)
        {
            content = new T[rows, cols];
            height = rows;
            width = cols;
        }

        public BidimensionalArray(int rows, int cols, T defaultValue)
            : this(rows, cols)
        {
            //content = new T[rows, cols];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    content[r, c] = defaultValue;
                }
            }
        }

        public BidimensionalArray(int rows, int cols, Instantiator instantiator)
            : this(rows, cols)
        {
            //content = new T[rows, cols];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    content[r, c] = instantiator();
                }
            }
        }

        public BidimensionalArray(int rows, int cols, CoordInstantiator instantiator)
            : this(rows, cols)
        {
            //content = new T[rows, cols];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    content[r, c] = instantiator(new Coord(c, r));
                }
            }
        }

        public BidimensionalArray(SerializationInfo info, StreamingContext context)
        {
            content = (T[,])info.GetValue(contentSerializableName, typeof(T[,]));
            height = (int)info.GetValue(heightSerializableName, typeof(int));
            width = (int)info.GetValue(widthSerializableName, typeof(int));
        }
        #endregion

        #region ITERATORS
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
        #endregion

        #region METHODS
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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(contentSerializableName, content, typeof(T[,]));
            info.AddValue(heightSerializableName, height, typeof(int));
            info.AddValue(widthSerializableName, width, typeof(int));
        }

        public T First(Func<T, bool> p)
        {
            for (int r = 0; r < this.Rows; r++)
            {
                for (int c = 0; c < this.Cols; c++)
                {
                    if(p(content[r,c]))
                    {
                        return content[r, c];
                    }
                }
            }

            return default(T);
        }
        #endregion
    }
}
