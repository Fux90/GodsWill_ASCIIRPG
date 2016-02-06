using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodsWill_ASCIIRPG
{
	public class Menu : IEnumerable
	{
        List<MenuItem> items;
        List<IMenuViewer> menuViewers;

        private int selectedEntry;
        public int SelectedEntry
        {
            get
            {
                return selectedEntry;
            }

            private set
            {
                selectedEntry = value;
                NotifyChangeSelection();
            }
        }

        public Menu()
        {
            items = new List<MenuItem>();
            menuViewers = new List<IMenuViewer>();
            SelectedEntry = -1;
        }

        public MenuItem this[int index]
        {
            get
            {
                return items[index];
            }
        }

        public void AddMenuItem(MenuItem menuItem)
        {
            items.Add(menuItem);
            if(SelectedEntry == -1)
            {
                SelectedEntry = 0;
            }

            NotifyLabels();
        }

        public void SelectPreviousItem()
        {
            if (SelectedEntry != -1)
            {
                SelectedEntry = Math.Max(0, SelectedEntry - 1);
            }
        }

        public void SelectNextItem()
        {
            if (SelectedEntry != -1)
            {
                SelectedEntry = Math.Min(SelectedEntry + 1, items.Count - 1);
            }
        }

        private void NotifyLabels()
        {
            var labels = items.Select(mI => mI.Label).ToArray();
            menuViewers.ForEach(mV => mV.NotifyLabels(labels));
        }

        private void NotifyChangeSelection()
        {
            menuViewers.ForEach(mV => mV.NotifyChangeSelection(SelectedEntry));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public MenuEnum GetEnumerator()
        {
            return new MenuEnum(items.ToArray());
        }

        public void ExecuteSelected(object parameters = null)
        {
            if (SelectedEntry != -1)
            {
                this[SelectedEntry].Action(parameters);
            }
        }
    }

    public class MenuItem
    {
        public delegate void MenuAction(object parameters);

        public string Label { get; private set; }
        public MenuAction Action { get; private set; }

        public MenuItem(string label,
                        MenuAction action)
        {
            Label = label;
            Action = action;
        } 
    }

    public class MenuEnum : IEnumerator
    {
        MenuItem[] items;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int position = -1;

        public MenuEnum(MenuItem[] list)
        {
            items = list;
        }

        public bool MoveNext()
        {
            position++;
            return (position < items.Length);
        }

        public void Reset()
        {
            position = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public MenuItem Current
        {
            get
            {
                try
                {
                    return items[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}