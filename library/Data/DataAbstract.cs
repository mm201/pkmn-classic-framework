using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Freemon.Data
{
    class DataAbstract
    {
        public virtual void AbilitiesFromVeekun(DataTable abilities);
        public virtual void ItemsFromVeekun(DataTable items, DataTable berries, DataTable machines);
        public virtual void MovesFromVeekun(DataTable moves);
        public virtual void PokemonFromVeekun(DataTable pokemon);
    }
}
