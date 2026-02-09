using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Program
    {
        static void Main(string[] args)
        {
            

            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,ProtocolType.Tcp);
            IPEndPoint serverEPTcp = new IPEndPoint(IPAddress.Any, 50001);

            serverSocket.Bind(serverEPTcp);
            serverSocket.Blocking = false;
            //int brojKorisnika = 4;
            

            Socket serverSocketUdp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            serverSocketUdp.Bind(new IPEndPoint(IPAddress.Any, 50032));
            serverSocketUdp.Blocking = false;

            List<Socket> korisnici = new List<Socket>();
            List<Korisnik> Igraci = new List<Korisnik>();
            Stopwatch stopwatch = new Stopwatch();

            
            //List<IPAddress> IPAdreseIgraca = new List<IPAddress>();
            List<string> Usernamovi = new List<string>();

            byte[] buffer = new byte[1024];
            bool tacno = false;
            EndPoint posiljaocEP = new IPEndPoint(IPAddress.Any, 0);
            List<IPEndPoint> IgraciEP = new List<IPEndPoint>();
            int velicinatable = 0;
            int brojIgraca = -1;

            do
            {
                Console.Write("\nUnesite broj igraca : ");
                tacno = int.TryParse(Console.ReadLine(), out brojIgraca);
            } while (brojIgraca < 2 || brojIgraca > 4 || !tacno);

            do
            {
                Console.Write("\nUnesite velicinu table : ");
                tacno = int.TryParse(Console.ReadLine(), out velicinatable);
            } while (velicinatable % brojIgraca != 0 || velicinatable < 16 || !tacno);
            
            for (int i = 0; i < brojIgraca; i++)
                Igraci.Add(null);
            serverSocket.Listen(brojIgraca);


            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            string clientPath = Path.Combine(
                baseDir,
                @"..\..\..\UDPClient\bin\Debug\UDPClient.exe"
            );

            clientPath = Path.GetFullPath(clientPath);

            for(int i = 0; i < brojIgraca; i++)
                Process.Start(clientPath);

            try
            {

                #region TCP povezivanje
                while (true) 
                {
                    List<Socket> checkRead = new List<Socket>();
                    List<Socket> checkError = new List<Socket>();

                    if (korisnici.Count < brojIgraca)
                    {
                        checkRead.Add(serverSocket);

                    }

                    checkError.Add(serverSocket);

                    foreach (Socket s in korisnici)
                    {
                        checkRead.Add(s);
                        checkError.Add(s);
                    }


                    Socket.Select(checkRead, null, checkError, 1000);

                    if(checkRead.Count > 0)
                    {
                        
                        foreach (Socket s in checkRead)
                        {
                            if(s == serverSocket)
                            {
                                Socket client = serverSocket.Accept();
                                client.Blocking = false;
                                korisnici.Add(client);
                                Console.WriteLine($"Klijent se povezao sa {client.RemoteEndPoint}");

                            }
                            else
                            {
                                int brBajta = s.Receive(buffer);
                                if (brBajta == 0)
                                {
                                    Console.WriteLine("Korisnik je prekinuo komunikaciju");
                                    s.Close();
                                    korisnici.Remove(s);

                                    continue;
                                }
                                else
                                {
                                    

                                    string username = Encoding.UTF8.GetString(buffer, 0, brBajta).Trim()                                    ;

                                    //IPEndPoint remoteEP = s.RemoteEndPoint as IPEndPoint;
                                    //IPAddress ip = remoteEP.Address;
                                    //IPAdreseIgraca.Add(ip);
                                    Usernamovi.Add(username);

                                    Console.WriteLine($"Prijavljen korisnik: {username} :: {s.RemoteEndPoint.ToString()}");

                                    s.Send(Encoding.UTF8.GetBytes("50032"));

                                }
                            }
                        }

                    }
                    if(Usernamovi.Count  == brojIgraca)
                    {
                        break;
                    }
                    
                    checkRead.Clear();
                }
                #endregion
                #region UDP povezivanje
                for (int i = 0; i < brojIgraca; i++)
                    IgraciEP.Add(null);

                byte[] prijemniBaffer = new byte[1024];
                int brojacZaKraj = 0;
                while (brojacZaKraj < brojIgraca)
                {
                    List<Socket> checkReadUdp = new List<Socket>{serverSocketUdp };
                    List<Socket> checkErrorUdp = new List<Socket> { serverSocketUdp};
                    Socket.Select(checkReadUdp, null, checkErrorUdp, 1000 * 1000 * 1000);

                    
                    if(checkReadUdp.Count > 0)
                    {

                        foreach (Socket s in checkReadUdp)
                        {
                            int brojBajtovaPoruke = s.ReceiveFrom(prijemniBaffer, ref posiljaocEP);
                            string primljenaPoruka = Encoding.UTF8.GetString(prijemniBaffer, 0, brojBajtovaPoruke).Trim();
                            // primljenaPoruka = primljenaPoruka.Trim();

                            if (stopwatch.IsRunning && stopwatch.ElapsedMilliseconds > 60 * 1000)
                                break;

                            Console.WriteLine($"Primljena poruka: {primljenaPoruka}\n Socket posiljaoca: {posiljaocEP}");

                            if (primljenaPoruka.StartsWith("HELLO|"))
                            {
                                string username = primljenaPoruka.Substring("HELLO|".Length);
                                Console.WriteLine(username);
                                int idx = Usernamovi.IndexOf(username);

                                stopwatch.Restart();

                                if (idx == -1)
                                {
                                    Console.WriteLine("HELLO za nepoznat username!");
                                    continue;
                                }

                                if (IgraciEP[idx] == null)
                                    brojacZaKraj++;

                                IgraciEP[idx] = (IPEndPoint)posiljaocEP;

                            }
                        }
                            
                    }

                }
                stopwatch.Stop();
                #endregion

                #region Inicijalizacija igraca, njihovih pozicija
                int pocetnaPozicijaIgraca = velicinatable / brojIgraca;

                for(int i = 0; i < brojIgraca; i++)
                {
                    string poruka = $"Potvrda povezivanja {Usernamovi[i]} sa serverom.";
                    byte[] bytes = Encoding.UTF8.GetBytes(poruka);
                    serverSocketUdp.SendTo(bytes, IgraciEP[i]);

                    Igraci[i] = new Korisnik(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + i, Usernamovi[i], pocetnaPozicijaIgraca * i, velicinatable);
                }
                #endregion
                //Console.ReadLine();
            }
            catch (SocketException ex) { 
                Console.WriteLine($"Doslo je do greske {ex}");
            }
            #region IGRA
            try
            {
                // obavestenje svih o pocetku igre
                Obavestenje_svih(IgraciEP, serverSocketUdp, brojIgraca);

                serverSocketUdp.Blocking = true;
                int igrac_na_redu = 0;
                string poruka_o_bacanju = "\tOBAVESTENJE|SERVER : Molimo bacite kockicu.";//treba da dodamo o bavestenje| ispred ovog stringa da znamo kad je novo bacanje kockice u klijentu
                byte[] porukaBuffer = Encoding.UTF8.GetBytes(poruka_o_bacanju);
                byte[] primljenaPoruka_buffer = new byte[1024];

                EndPoint ep = new IPEndPoint(IPAddress.Any, 0);

                while (!Kraj(Igraci, IgraciEP, serverSocketUdp))
                {
                    Console.WriteLine($"\tNa potezu igrac : {Igraci[igrac_na_redu].Username}");
                    serverSocketUdp.SendTo(porukaBuffer, IgraciEP[igrac_na_redu]);
                    int primljeno = serverSocketUdp.ReceiveFrom(primljenaPoruka_buffer, ref ep);
                    if (ep.Equals(IgraciEP[igrac_na_redu]))
                    {
                        string broj_kockice_str = Encoding.UTF8.GetString(primljenaPoruka_buffer, 0, primljeno).Trim();
                        int br_kockice = int.Parse(broj_kockice_str);
                        //Console.WriteLine("DEBUG        " + br_kockice);
                        // racunanje poteza
                        List<Potez> moguci_potezi = Igraci[igrac_na_redu].MoguciPotezi(br_kockice, velicinatable);
                        byte[] dataBuffer = new byte[2048];
                        using (MemoryStream ms = new MemoryStream())
                        {
                            BinaryFormatter bf = new BinaryFormatter();
                            bf.Serialize(ms, moguci_potezi);
                            dataBuffer = ms.ToArray();
                        }
                        serverSocketUdp.SendTo(dataBuffer, IgraciEP[igrac_na_redu]); // poslao moguce poteze

                        //treba da odigramo potez

                        byte[] prijemIzabraneopcije = new byte[10];
                        serverSocketUdp.ReceiveFrom(prijemIzabraneopcije, ref ep);
                        int opcija = int.Parse(Encoding.UTF8.GetString(prijemIzabraneopcije).Trim());
                        //Console.WriteLine($" DEBUG {opcija}");
                        //TREBA DA POZOVEMO FUNKCIJU ZA IGRANJE POTEZA
                        if(opcija != -1)
                        {
                            OdigrajPotez(moguci_potezi[opcija], Igraci[igrac_na_redu], ref Igraci, velicinatable, IgraciEP, serverSocketUdp);
                            PomocneFunkcije pf = new PomocneFunkcije();
                            byte[] nizBajtova = Encoding.UTF8.GetBytes(pf.IspisIzvestaja(Igraci));

                            for (int i = 0; i < Igraci.Count; i++)
                            {
                                serverSocketUdp.SendTo(nizBajtova, IgraciEP[i]);
                            }
                            Console.WriteLine(pf.IspisIzvestaja(Igraci));
                            
                            

                        }
                        if (br_kockice == 6)    // ako je igrac dobio 6
                            continue;           // ponovo igra
                    }

                    igrac_na_redu = (igrac_na_redu + 1) % brojIgraca; // vrti se po modulu broja igraca
                } // treba da implementiram proveru uslova

            }catch (SocketException ex)
            {
                Console.WriteLine($"Doslo je do greske :: {ex.Message}");
            }
        #endregion

        }
        #region FUNKCIJE
        static bool Kraj(List<Korisnik> igraci, List<IPEndPoint> igraciEP, Socket serverSocket)// nije zavrsena funkcija
        {
            foreach (Korisnik igrac in igraci)
            {
                int u_kucici = 0;
                for (int i = 0; i < igrac.Figure.Count; i++)
                {
                    if (igrac.Figure[i].Status == false && igrac.Figure[i].Do_cilja == 0)
                        u_kucici++;
                }

                if (u_kucici == 4)
                {
                    Obavesti_o_kraju(igrac.Username, igraciEP, serverSocket, igraci);
                   return true;// ako ima 4 figure u kucici kraj igre
                }
                   
            }
            return false;
        }

        static void Obavesti_o_kraju(string username, List<IPEndPoint> IgraciEP, Socket serverSocket, List<Korisnik> igraci)
        {
            string obavestenje = $"POBEDA|Pobedio je igrac {username}\n";
            PomocneFunkcije pf = new PomocneFunkcije();
            obavestenje += pf.IspisIzvestaja(igraci);
            foreach(IPEndPoint ep in IgraciEP)
            {
                serverSocket.SendTo(Encoding.UTF8.GetBytes(obavestenje), ep);
            }
            Console.WriteLine(obavestenje);
        }

        static bool Obavestenje_svih(List<IPEndPoint> igraci, Socket serverSocket,int broj_igraca)
        {
            try
            {
                string obavestenje = "============================  IGRA JE POCELA  ============================\n";
                byte[] obavestenjeBuffer = Encoding.UTF8.GetBytes(obavestenje);

                Console.WriteLine(obavestenje);
                for (int i = 0; i < broj_igraca; i++)
                {
                    int br_byte = serverSocket.SendTo(obavestenjeBuffer, igraci[i]);
                    if (br_byte == 0)
                        return false;
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return true;
        }

        static bool OdigrajPotez(Potez potez, Korisnik igrac, ref List<Korisnik> igraci,int velicinaTable, List<IPEndPoint> igraciEP, Socket ServerSocket)
        {
            try
            {
                int pozicija = -1;
                int indexOnogKoJeOdigraoPotez = -1;
                string porukaPojeden = $"{igrac.Username} je pojeo ";

                string odigranPotez = "Potez je odigran\n";
                byte[] buffer = Encoding.UTF8.GetBytes(odigranPotez);

                foreach (Korisnik i in igraci)
                {

                    if (igrac.ID == i.ID)
                    {
                        indexOnogKoJeOdigraoPotez = igraci.IndexOf(i);
                        Console.WriteLine($"Potez odigrao {i.Username}");
                        odigranPotez += $" {i.Username}";
                        if (potez.Akcija == TipAkcije.AKTIVACIJA)
                        {
                            for (int j = 0; j < 4; j++)
                            {

                                if (potez._Figura.ID == i.Figure[j].ID)
                                {
                                    i.Figure[j].Status = true;
                                    pozicija = i.Figure[j].Pozicija = i.Start;
                                    i.Figure[j].Do_cilja = velicinaTable - 2;
                                }
                            }

                        }
                        else
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                if (potez._Figura.ID == i.Figure[j].ID)
                                {
                                    pozicija = i.Figure[j].Pozicija = (i.Figure[j].Pozicija + potez.Br_polja) % velicinaTable;
                                    i.Figure[j].Do_cilja -= potez.Br_polja;

                                    if (i.Figure[j].Do_cilja == 0)
                                    {
                                        i.Figure[j].Status = false;
                                        string poruka = "USLI STE U CILJ";
                                        ServerSocket.SendTo(Encoding.UTF8.GetBytes(poruka), igraciEP[indexOnogKoJeOdigraoPotez]);

                                    }
                                }
                            }
                        }
                    }
                }

                if (pozicija != -1)
                {
                    foreach (Korisnik i in igraci)
                    {
                        if (igrac.ID != i.ID)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                if (i.Figure[j].Pozicija == pozicija && i.Figure[j].Status == true)
                                {
                                    i.Figure[j].Do_cilja = velicinaTable - 2;
                                    i.Figure[j].Status = false;
                                    i.Figure[j].Pozicija = -1;
                                    porukaPojeden = porukaPojeden + i.Username;

                                    Console.WriteLine(porukaPojeden);
                                    byte[] bufferPojeden = Encoding.UTF8.GetBytes(porukaPojeden);
                                    ServerSocket.SendTo(bufferPojeden, igraciEP[igraci.IndexOf(i)]);
                                    ServerSocket.SendTo(bufferPojeden, igraciEP[indexOnogKoJeOdigraoPotez]);
                                    return true;
                                }
                            }
                        }
                    }
                }

                Console.WriteLine($"\t{odigranPotez}");
                ServerSocket.SendTo(buffer, igraciEP[indexOnogKoJeOdigraoPotez]);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            
        }
        #endregion

    }
}
