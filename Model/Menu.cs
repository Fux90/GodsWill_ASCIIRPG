using GodsWill_ASCIIRPG.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GodsWill_ASCIIRPG
{
	public abstract class Menu : IEnumerable, IViewable<IMenuViewer>
	{
        List<MenuItem> items;
        List<IMenuViewer> menuViewers;

        private string title;
        public string Title
        {
            get
            {
                if(title == null)
                {
                    title = "";
                }

                return title;
            }

            protected set
            {
                title = value;
                NotifyTitleChange();
            }
        }

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
            var actives = items.Select(mI => mI.Active).ToArray();
            menuViewers.ForEach(mV => mV.NotifyLabels(labels, actives));
        }

        private void NotifyChangeSelection()
        {
            menuViewers.ForEach(mV => mV.NotifyChangeSelection(SelectedEntry));
        }

        private void NotifyTitleChange()
        {
            var t = Title;
            menuViewers.ForEach(mV => mV.NotifyTitleChange(t));
        }

        public void NotifyAll()
        {
            NotifyLabels();
            NotifyTitleChange();
            NotifyChangeSelection();
        }

        public void RegisterView(IMenuViewer viewer)
        {
            this.menuViewers.Add(viewer);

            NotifyAll();
        }

        public void Open()
        {
            if(menuViewers.Count > 0)
            {
                menuViewers[0].UpmostBring();
            }
        }

        public void Close()
        {
            if (menuViewers.Count > 0)
            {
                menuViewers[0].Close();
            }
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
                var entry = this[SelectedEntry];
                if (entry.Active)
                {
                    entry.Action(parameters);
                }
            }
        }

        protected void ActivateByName(string name)
        {
            var mItem = this.items.Where(mI => mI.Label == name).FirstOrDefault();
            if(mItem != null)
            {
                mItem.Activate();
            }
        }
    }

    public class MenuItem
    {
        public delegate void MenuAction(object parameters);

        public string Label { get; private set; }
        public bool Active { get; private set; }
        public MenuAction Action { get; private set; }

        public MenuItem(string label,
                        MenuAction action,
                        bool active = true)
        {
            Label = label;
            Active = active;
            Action = action;
        }

        public void Activate()
        {
            Active = true;
        }

        public void Deactivate()
        {
            Active = false;
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