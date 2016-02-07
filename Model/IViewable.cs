using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    public interface IViewable<TViewer>
        where TViewer : IViewer
    {
        void RegisterView(TViewer viewer);
    }
}
