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
            int brojKorisnika = 4;
            serverSocket.Listen(brojKorisnika);

            Socket serverSocketUdp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            serverSocketUdp.Bind(new IPEndPoint(IPAddress.Any, 50032));
            serverSocketUdp.Blocking = false;



            List<Socket> korisnici = new List<Socket>();

            List<Korisnik> Igraci = new List<Korisnik>();

            List<IPAddress> IPAdreseIgraca = new List<IPAddress>();

            List<string> Usernamovi = new List<string>();

            byte[] buffer = new byte[1024];

            bool kraj = false;

            EndPoint posiljaocEP = new IPEndPoint(IPAddress.Any, 0);

            List<IPEndPoint> IgraciEP = new List<IPEndPoint>();
            int velicinatable = 0;
            do
            {
                Console.WriteLine("Unesite velicinu table");
                velicinatable = int.Parse(Console.ReadLine());
            } while (velicinatable %4 != 0 || velicinatable < 16);

            try
            {

                while (true) 
                {
                    List<Socket> checkRead = new List<Socket>();
                    List<Socket> checkError = new List<Socket>();

                    if (korisnici.Count < brojKorisnika)
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

                                    IPEndPoint remoteEP = s.RemoteEndPoint as IPEndPoint;
                                    IPAddress ip = remoteEP.Address;
                                    IPAdreseIgraca.Add(ip);
                                    Usernamovi.Add(username);

                                    Console.WriteLine($"Prijavljen korisnik: {username}, IP: {ip}");

                                    s.Send(Encoding.UTF8.GetBytes("50032"));

                                }
                            }

                           
                      
                        }

                    }
                    if(Usernamovi.Count  == brojKorisnika)
                    {
                        break;
                    }
                    
                    checkRead.Clear();
                }

                for (int i = 0; i < brojKorisnika; i++)
                    IgraciEP.Add(null);

                byte[] prijemniBaffer = new byte[1024];
                int brojacZaKraj = 0;
                while (brojacZaKraj < 4)
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
                            Console.WriteLine($"Primljena poruka: {primljenaPoruka}\n Socket posiljaoca: {posiljaocEP}");

                            if (primljenaPoruka.StartsWith("HELLO|"))
                            {
                                string username = primljenaPoruka.Substring("HELLO|".Length);
                                Console.WriteLine(username);
                                int idx = Usernamovi.IndexOf(username);

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


                int pocetnaPozicijaIgraca = velicinatable / 4;

                for(int i = 0; i < 4; i++)
                {
                    string poruka = $"Vas username je {Usernamovi[i]}";
                    byte[] bytes = Encoding.UTF8.GetBytes(poruka);
                    serverSocketUdp.SendTo(bytes, IgraciEP[i]);

                    Igraci[i] = new Korisnik(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), Usernamovi[i], pocetnaPozicijaIgraca * i);
                }



                Console.ReadLine();

            }
            catch (SocketException ex) { 
                Console.WriteLine($"Doslo je do greske {ex}");
            
            }
        }
    }
}
