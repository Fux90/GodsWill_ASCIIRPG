using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodsWill_ASCIIRPG
{
    /// <summary>
    /// Menu Viewers
    /// </summary>
	public interface IMenuViewer : IViewer
	{
        void NotifyLabels(string[] menuLabels, bool[] activeLabels);
        void NotifyChangeSelection(int selectedIndex);
        void NotifyTitleChange(string title);
        void UpmostBring();
        void Close();
	}
}