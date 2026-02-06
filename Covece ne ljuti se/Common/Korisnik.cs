using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [Serializable]
    public class Korisnik
    {
        private long id;
        private string username;
        //private TipBoje boja;
        private List<Figura> figure;
        private int start;
        private int cilj;

        //long id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        public long ID
        {
            get { return id; } 
            set { id = value; }
        }
        public string Username
        {
            get { return username; }
            set { username = value; }
        }
        public int Start
        {
            get { return start; }
            set { start = value; }
        }
        public int Cilj
        {
            get { return cilj; }
            set { cilj = value; }
        }/*
        public TipBoje Boja
        {
            get { return boja; }
            set { boja = value; }
        }*/
        public List<Figura> Figure
        {
            get { return figure; }
            set { figure = value; }
        }

        public Korisnik()
        {
            ID = 0;
            Username = string.Empty;
            //Boja = 0;
            Figure = new List<Figura>(4);
            Start = 0;
            Cilj = 0;
        }
        public Korisnik(long id, string username, int start, int velicina_table)
        {
            ID = id;
            Username = username;
           // Boja = boja;
            Start = start;
            Figure = new List<Figura>(4);
            Cilj = (Start + velicina_table - 2) % velicina_table;

            for(int i = 0; i < 4; i++)
            {
                Figure.Add(new Figura(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + i,velicina_table-2));
            }
        }

        public List<Potez> MoguciPotezi(int br_kockice, int velicina_table)
        {
            List<Potez> potezi = new List<Potez>();
            
            foreach(Figura f in Figure)
            {
                int narednaPozicija = -1, poz = -1;
                if (br_kockice == 6 && f.Status == false && f.Pozicija == -1)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (Start == Figure[i].Pozicija)
                            poz = Start;
                    }
                    if(poz == -1)
                        potezi.Add(new Potez(f, TipAkcije.AKTIVACIJA, 0));
                }
                else if(f.Status == true && (f.Do_cilja - br_kockice) >= 0)
                {
                    for (int i = 0; i < 4; i++) 
                    {
                        if ((f.Pozicija + br_kockice) % velicina_table == Figure[i].Pozicija)
                            narednaPozicija = Figure[i].Pozicija;
                    }
                    if(narednaPozicija == -1)
                        potezi.Add(new Potez(f, TipAkcije.POMERANJE, br_kockice));
                }
            }
            return potezi;
        }

        
    }

    
}
