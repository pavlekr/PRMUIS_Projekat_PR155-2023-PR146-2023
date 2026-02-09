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
            string usernamovi = string.Empty;
            for (int i = 0; i < Igraci.Count * 23; i++)
                usernamovi += "-";
            usernamovi += "\n";

            usernamovi += "| "; 
            foreach(Korisnik igrac in Igraci)
            {
                usernamovi += $"{igrac.Username,-20} |";
            }
            usernamovi += "\n";

            for (int i = 0; i < Igraci.Count * 23; i++)
                usernamovi += "-";
            usernamovi += "\n";

            string figure = string.Empty;
            for (int i = 0; i < 4; i++) 
            {
                figure += "| ";
                foreach (Korisnik igrac in Igraci)
                {
                    figure += $"{igrac.Figure[i].ToString(),-20} |";
                }
                figure += "\n";
            }

            for (int i = 0; i < Igraci.Count * 23; i++)
                figure += "-";
            figure += "\n";

            return usernamovi + figure;
        }
    }
    
}
