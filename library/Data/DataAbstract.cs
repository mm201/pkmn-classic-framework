using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Freemon.Data
{
    abstract class DataAbstract
    {
        public abstract void AbilitiesFromVeekun(DataTable abilities);
        public abstract void ItemsFromVeekun(DataTable items, DataTable berries, DataTable machines);
        public abstract void MovesFromVeekun(DataTable moves);
        public abstract void PokemonFromVeekun(DataTable pokemon);
    }
}
