﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GodsWill_ASCIIRPG.UIControls
{
    public partial class PagedListUserControl<T> : UserControl
    {
        public const float FontSize = 10.0f;

        private bool showSelection;

        public int SelectedIndex { get; private set; }
        public int SelectedPage { get; private set; }
        public int PagesCount
        {
            get
            {
                var rowsPerPage = RowsPerPage;
                var pgCount = (int)Math.Ceiling((float)items.Length / (float)rowsPerPage);
                SelectedPage = Math.Min(SelectedPage, pgCount - 1);
                var pgOfSelectedIx = (int)Math.Floor((float)SelectedIndex / (float)rowsPerPage);
                /*
                    On shrinking, select the last element in page
                */
                if(pgOfSelectedIx > SelectedPage)
                {
                    SelectedIndex = Math.Max(0, (SelectedPage + 1) * rowsPerPage - 1);
                }
                return pgCount;
            }
        }
        public int RowsPerPage { get { return (int)Math.Floor((float)this.Height / (float)this.FontHeight); } }

        public delegate string StringifyMethod(T element);
        private StringifyMethod stringify = (element) => element.ToString();
        public StringifyMethod Stringify
        {
            get { return stringify; }
            set { stringify = value; }
        }

        private T[] items;
        public T[] Items
        {
            get
            {
                return items == null ? new T[] { } : items;
            }

            set
            {
                items = value;
                SelectedIndex = items == null || items.Length == 0 ? -1 : 0;
                SelectedPage = 0;
            }
        }

        public PagedListUserControl()
        {
            InitializeComponent();
            SelectedIndex = -1;

            this.DoubleBuffered = true;
            this.BackColor = Color.Black;
            this.Font = new Font(FontFamily.GenericMonospace, FontSize);

            this.showSelection = true;
        }

        public void SelectPrevious()
        {
            SelectedIndex = items == null || items.Length == 0 ? -1 : Math.Max(0, SelectedIndex - 1);
            SelectedPage = SelectedIndex / RowsPerPage;
        }

        public void SelectNext()
        {
            SelectedIndex = items == null || items.Length == 0 ? -1 : Math.Min(items.Length - 1, SelectedIndex + 1);
            SelectedPage = SelectedIndex / RowsPerPage;
        }

        public void SelectNextPage()
        {
            if (SelectedPage < PagesCount - 1)
            {
                SelectedPage = SelectedPage + 1;
                SelectedIndex = Math.Min(items.Length - 1, SelectedIndex + RowsPerPage);
            }
        }

        public void SelectPreviousPage()
        {
            if (SelectedPage > 0)
            {
                SelectedPage = SelectedPage - 1;
                SelectedIndex = Math.Max(0, SelectedIndex - RowsPerPage);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if(SelectedIndex != -1)
            {
                var g = e.Graphics;
                var indexToShow = SelectedPage * RowsPerPage;
                var finalIndex = Math.Min(items.Length, indexToShow + RowsPerPage);

                var pos = new PointF();
                for (int r = indexToShow; r < finalIndex; r++, indexToShow++)
                {
                    if(showSelection && SelectedIndex == indexToShow)
                    {
                        g.FillRectangle(Brushes.Blue, new RectangleF(pos, new SizeF(this.Width, FontHeight)));
                    }
                    g.DrawString(   Stringify(items[indexToShow]), 
                                    Font, 
                                    Brushes.White, 
                                    pos);
                    pos.Y += Font.Height;
                }
            }
        }

        public void HideSelection()
        {
            showSelection = false;
        }

        public void ShowSelection()
        {
            showSelection = true;
        }
    }
}
