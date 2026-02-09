using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{ 
    [Serializable]
    public class Figura
    {
        long id;
        private int pozicija;
        private bool status; // aktivna/neaktivna
        private int do_cilja;

        public long ID
        {
            get { return id; }
            set { id = value; }
        }
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
            ID = 0;
            Pozicija = -1;
            Status = false;
            Do_cilja = 0;
        }

        public Figura(long id, int br_polja)
        {
            ID = id;
            Pozicija = -1;
            Do_cilja = br_polja;
            Status = false;
        }

        public override string ToString()
        {
            string rez = string.Empty;
            if (Do_cilja == 0 && Status == false)
            {
                rez = "Pijun: U CILJU";
            }
            else if (Status == false && Pozicija == -1)
            {
                rez = "Pijun: U KUCICI";
            }
            else if(Status  == true) 
            {
                rez = $"Pijun: [{Pozicija}], ({Do_cilja})";
            }
            return rez;
        }
    }
}
