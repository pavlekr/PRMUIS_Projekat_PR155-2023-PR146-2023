using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class PomocneFunkcije
    {
        public  string IspisIzvestaja(List<Korisnik> Igraci)
        {
            string usernamovi = "| "; 
            foreach(Korisnik igrac in Igraci)
            {
                usernamovi += $"{igrac.Username,-20} |";
            }
            usernamovi += "\n";

            string figure = "| ";
            for (int i = 0; i < 4; i++) 
            {
                foreach(Korisnik igrac in Igraci)
                {
                    figure += $"{igrac.Figure[i].ToString(),-20} |";
                }
                figure += "\n| ";
            }
            return usernamovi + figure;
        }
    }
    
}
