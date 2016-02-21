﻿using GodsWill_ASCIIRPG.Model.Edibles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    public delegate void Ate(AteEventArgs e);

    public interface IEater
    {
        int Hunger { get; }
        void Eat(Edible edible);

        event Ate Ate;
    }
}
