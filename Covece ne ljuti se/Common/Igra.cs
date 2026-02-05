using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Igra
    {
        public StatusIgre Status { get; set; }
        public Igra()
        {
            Status = StatusIgre.POCETAK_IGRE;
        }
    }
}
