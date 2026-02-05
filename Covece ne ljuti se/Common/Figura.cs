using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Figura
    {
        private int pozicija;
        private bool status;
        private int do_cilja;

        public int Pozicija
        {
            get {  return pozicija; }
            set { pozicija = value; }
        }
        public bool Status
        {
            get { return status; }
            set { status = value; }
        }
        
        public int Do_cilja
        {
            get { return do_cilja; }
            set { do_cilja = value; }
        }

        public Figura()
        {
            Pozicija = -1;
            Status = false;
            Do_cilja = 0;
        }

        public Figura(int br_polja)
        {
            Pozicija = -1;
            Do_cilja = br_polja;
            Status = false;
        }

        public override string ToString()
        {
            return $"Pijun: {Pozicija}, ({Do_cilja})";
        }
    }
}
