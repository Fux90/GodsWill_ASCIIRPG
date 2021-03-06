﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.Gods
{
    public interface IPrayer
    {
        Pg.Level CurrentLevel { get; }
        void Pray(out bool acted);
        void RegisterTemporaryMod(TemporaryModifier mod);
    }
}
