using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GodsWill_ASCIIRPG.Model;

namespace GodsWill_ASCIIRPG
{
	public interface IBackpackViewer : IViewer
	{
        void NotifyAdd(Item[] items);
	}
}