using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Pijun
    {
        private int pozicija;
        private bool u_kucici;
        private bool u_cilju;
        private int do_cilja;

        public int Pozicija
        {
            get {  return pozicija; }
            set { pozicija = value; }
        }
        public bool U_kucici
        {
            get { return u_kucici; }
            set { u_kucici = value; }
        }
        public bool U_cilju
        {
            get { return u_cilju; }
            set { u_cilju = value; }
        }
        public int Do_cilja
        {
            get { return do_cilja; }
            set { do_cilja = value; }
        }

        public Pijun()
        {
            Pozicija = -1;
            U_kucici = true;
            U_cilju = false;
            Do_cilja = 0;
        }

        public Pijun(int br_polja)
        {
            Pozicija = -1;
            Do_cilja = br_polja;
            U_kucici = true;
            U_cilju = false;
        }

        public override string ToString()
        {
            return $"Pijun: {Pozicija}, ({Do_cilja})";
        }
    }
}
