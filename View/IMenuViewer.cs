using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodsWill_ASCIIRPG
{
	public interface IMenuViewer : IViewer
	{
        void NotifyLabels(string[] menuLabels);
        void NotifyChangeSelection(int selectedIndex);
	}
}