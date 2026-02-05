using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum TipBoje
    {
        CRVENA,
        PLAVA,
        ZUTA,
        ZELENA
    }

    public enum TipAkcije
    {
        AKTIVACIJA,
        DEAKTIVACIJA,
        POMERANJE
    }

    public enum StatusIgre
    {
        POCETAK_IGRE,
        IGRAC_NA_POTEZU,
        POZICIJE_SVIH_FIGURA,
        ZAVRSETAK_IGRE
    }
}
