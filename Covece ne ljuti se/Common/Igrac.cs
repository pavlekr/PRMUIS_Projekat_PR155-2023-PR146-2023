using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Igrac
    {
        private string username;
        private TipBoje boja;
        private List<Pijun> pijuni;
        int start;

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
        public TipBoje Boja
        {
            get { return boja; }
            set { boja = value; }
        }
        public List<Pijun> Pijuni
        {
            get { return pijuni; }
            set { pijuni = value; }
        }

        public Igrac()
        {
            Username = string.Empty;
            Boja = 0;
            Pijuni = new List<Pijun>(4);
            Start = 0;
        }
        public Igrac(string username, TipBoje boja, int start)
        {
            Username = username;
            Boja = boja;
            Start = start;
            Pijuni = new List<Pijun>(4);

            for(int i = 0; i < 4; i++)
            {
                Pijuni[i] = new Pijun(Start);
            }
        }
    }

    
}
