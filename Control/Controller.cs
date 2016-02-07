using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodsWill_ASCIIRPG
{
    public interface Controller
    {
        void UnregisterAll();
        void Notify(ControllerCommand cmd);
    }

	public interface Controller<T> : Controller
	{
        void Register(T model);
        void Unregister(T element);
	}
}