using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.View
{
    /// <summary>
    /// Atom Listener
    /// </summary>
    public interface IAtomListener : IViewer
    {
        /// <summary>
        /// Notify Listener with a message
        /// </summary>
        /// <param name="who">Who sent message</param>
        /// <param name="msg">What message has been sent</param>
        void NotifyMessage(Atom who, string msg);
        void CleanPreviousMessages();
    }

    /// <summary>
    /// An Atom Listener whose messages can be saved to or load from file
    /// </summary>
    public interface ISaveableAtomListener : IAtomListener
    {
        /// <summary>
        /// Save current received messages
        /// </summary>
        /// <param name="outputStream">Strem in which write message</param>
        void SaveMessages(Stream outputStream);

        /// <summary>
        /// Load messages from file
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns>Returns good reading</returns>
        bool LoadMessages(Stream inputStream);
    }

    /// <summary>
    /// Atom listener that stores Pg story
    /// </summary>
    public interface IPgStoryAtomListener
    {
        /// <summary>
        /// Saves to a text file all Pg story
        /// </summary>
        /// <param name="writer">TextWriter that writes the story</param>
        void SaveMessagesAsTxt(StreamWriter writer);
    }
}
