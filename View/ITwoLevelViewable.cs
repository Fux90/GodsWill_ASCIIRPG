using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.View
{
    public interface ITwoLevelViewable<TViewer, TContent>
    where TViewer : IViewer
    {
        void RegisterSecondaryView(TViewer viewer);
        void NotifySecondaryViewers(TContent content);
    }
}
