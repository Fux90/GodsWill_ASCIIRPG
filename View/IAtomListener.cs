using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.View
{
    public interface IAtomListener
    {
        void NotifyMessage(Atom who, string msg);
        void CleanPreviousMessages();
    }

    public interface ISaveableAtomListener : IAtomListener
    {
        void SaveMessages(Stream outputStream);
        bool LoadMessages(Stream inputStream);
    }

    public interface IPgStoryAtomListener
    {
        void SaveMessagesAsTxt(StreamWriter writer);
    }
}
