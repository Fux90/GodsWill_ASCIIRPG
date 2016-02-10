using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodsWill_ASCIIRPG
{
    /// <summary>
    /// Controller of AI Characters
    /// </summary>
	public interface AIController : Controller<AICharacter>
	{
        /// <summary>
        /// Register a list to characters to be controlled
        /// </summary>
        /// <param name="characters">Characters to be controlled</param>
        void RegisterAll(IEnumerable<AICharacter> characters);

        /// <summary>
        /// Unregister all characters controlled
        /// </summary>
        void RemoveAll();
	}
}