using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodsWill_ASCIIRPG
{
    public class MapController : Controller<Map>
    {
        Map controlledMap;

        public void Notify(ControllerCommand cmd)
        {
            if(controlledMap != null)
            {
                switch(cmd)
                {

                }
            }
        }

        public void Register(Map map)
        {
            controlledMap = map;
        }

        public void Unregister(Map map)
        {
            controlledMap = null;
        }

        public void UnregisterAll()
        {
            controlledMap = null;
        }
    }
}