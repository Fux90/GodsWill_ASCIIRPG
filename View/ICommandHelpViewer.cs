using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.View
{
    interface ICommandHelpViewer : IViewer
    {
        void UpmostBring();
        void Close();
    }
}
