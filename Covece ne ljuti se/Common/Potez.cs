using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Potez
    {
        private Figura _figura;
        private TipAkcije akcija;
        int br_polja;

        public Figura _Figura
        {
            get { return _figura; }
            set { _figura = value; }
        }
        public TipAkcije Akcija
        {
            get { return akcija; }
            set { akcija = value; }
        }
        public int Br_polja
        {
            get { return br_polja; }
            set { br_polja = value; }
        }

        public Potez()
        {
            _Figura = new Figura();
            Akcija = 0;
            Br_polja = 0;
        }
        public Potez(Figura figura, TipAkcije akcija, int br_polja)
        {
            _Figura = figura;
            Akcija = akcija;
            Br_polja = br_polja;
        }
    }
}
