using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodsWill_ASCIIRPG
{
    /// <summary>
    /// General controller interface
    /// </summary>
    public interface Controller
    {
        /// <summary>
        /// Unregister all controlled elements
        /// </summary>
        void UnregisterAll();

        /// <summary>
        /// Notifies all controlled elements
        /// </summary>
        /// <param name="cmd">Command to be notified</param>
        void Notify(ControllerCommand cmd);
    }

    /// <summary>
    /// Typed controller
    /// </summary>
    /// <typeparam name="T">Type of element that can be controlled</typeparam>
	public interface Controller<T> : Controller
	{
        /// <summary>
        /// Register an element to be controlled
        /// </summary>
        /// <param name="model">Element to be controlled</param>
        void Register(T model);

        /// <summary>
        /// Unregister certain element
        /// </summary>
        /// <param name="element">Element to unregister</param>
        void Unregister(T element);
	}
}