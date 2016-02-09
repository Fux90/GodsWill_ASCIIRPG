using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodsWill_ASCIIRPG
{
	public interface AIController : Controller<AICharacter>
	{
        void RegisterAll(IEnumerable<AICharacter> characters);
        void RemoveAll();
	}
}