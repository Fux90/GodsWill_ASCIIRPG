﻿using GodsWill_ASCIIRPG.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Control
{
    public interface BackpackController : Controller<Backpack>
    {
        void Notify(ControllerCommand cmd);
        void Register(Backpack backpack);
        void Unregister();
    }
}