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
        }
        public Korisnik(long id, string username, int start)
        {
            ID = id;
            Username = username;
           // Boja = boja;
            Start = start;
            Figure = new List<Figura>(4);

            for(int i = 0; i < 4; i++)
            {
                Figure[i] = new Figura(Start);
            }
        }
    }

    
}
