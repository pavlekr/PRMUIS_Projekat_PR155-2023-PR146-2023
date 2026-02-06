using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum TipBoje
    {
        CRVENA = 0,
        PLAVA = 1,
        ZUTA = 2,
        ZELENA = 3
    }

    public enum TipAkcije
    {
        AKTIVACIJA,
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
