using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodsWill_ASCIIRPG
{
	public interface Controller<T>
	{
        void Register(T model);
        void Unregister(T element);
        void Notify(ControllerCommand cmd);
	}
}