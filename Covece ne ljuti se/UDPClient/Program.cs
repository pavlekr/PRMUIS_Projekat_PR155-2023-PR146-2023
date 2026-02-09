using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Loopback, 50001);
            Socket SocketUdp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            EndPoint ServerUdpEP = new IPEndPoint(IPAddress.Loopback, 0);
            byte[] buffer = new byte[2048];

            #region TCP i UDP kontektovanje
            try
            {
                clientSocket.Connect(serverEP);
                Console.WriteLine("Korisnik je uspesno povezan sa serverom!");
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Greska pri povezivanju {ex.Message}!");
                return;
            }

            int brBajta = 0;
            try
            {
                Console.WriteLine("Unesite korisnicko ime: ");
                string username = Console.ReadLine();

                clientSocket.Send(Encoding.UTF8.GetBytes(username));
                clientSocket.Receive(buffer);
                string port = Encoding.UTF8.GetString(buffer);
                Console.WriteLine($"Port je: {port}");
                port = port.Trim();

                int udpPort = int.Parse(port);
                ServerUdpEP = new IPEndPoint(IPAddress.Loopback, udpPort);

                string poruka = "HELLO|" + username;
                byte[] data = Encoding.UTF8.GetBytes(poruka);
                SocketUdp.SendTo(data, ServerUdpEP);

                // prijem potvrde povezivanja
                SocketUdp.ReceiveFrom(buffer, ref ServerUdpEP);
                Console.WriteLine(Encoding.UTF8.GetString(buffer));
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Doslo je do greske {ex.Message}!");
            }
            #endregion

            SocketUdp.Blocking = true;
            // pocela igra obavestenje
            Console.WriteLine("@@@@@@@@@@");
            int n = SocketUdp.ReceiveFrom(buffer, ref ServerUdpEP);
            Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, n).Trim());
            do
            {
                //obavestenje da je igrac na potezu
                Console.WriteLine("##########");
                int br = SocketUdp.ReceiveFrom(buffer, ref ServerUdpEP);//TREBA DA NAPRAVIO DA PREPOZNAJEMO KAD JE OBAVESTENJE ZA BAcANJE KOCKICE A KAD JE NEKO DRUGO OBAVESTENJE
                string poruka = Encoding.UTF8.GetString(buffer, 0, br).Trim();
                Console.WriteLine("************");
                if (poruka.StartsWith("OBAVESTENJE|"))
                {
                    poruka = poruka.Substring("OBAVESTENJE|".Length);
                    Console.WriteLine(poruka);
                    string rez = "";
                    do
                    {
                        Console.WriteLine(" 1 - Baci kockicu");
                        rez = Console.ReadLine().Trim();
                    } while (rez != "1");

                    Random rand = new Random();
                    int br_kockice = 1 + rand.Next() % 6;
                    Console.WriteLine($"Dobili ste {br_kockice}");
                    byte[] slanje_buffer = Encoding.UTF8.GetBytes(br_kockice.ToString());
                    SocketUdp.SendTo(slanje_buffer, ServerUdpEP);

                    byte[] dataBuffer = new byte[2048];
                    List<Potez> moguci_potezi = new List<Potez>();
                    // primanje mogucih poteza
                    SocketUdp.Receive(dataBuffer);
                    using (MemoryStream ms = new MemoryStream(dataBuffer))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        moguci_potezi = (List<Potez>)bf.Deserialize(ms);
                    }
                    Console.WriteLine("\tMoguci potezi:");
                    if (moguci_potezi.Count > 0)
                    {
                        for (int i = 0; i < moguci_potezi.Count; i++)
                        {
                            Console.WriteLine($" {i} - {moguci_potezi[i]._Figura.ID} {moguci_potezi[i]._Figura.ToString()} {moguci_potezi[i].Akcija.ToString()} Pozicija : {moguci_potezi[i]._Figura.Pozicija} Do cilja: {moguci_potezi[i]._Figura.Do_cilja}");
                        }
                        int opcija = -1;
                        bool validno = false;
                        do
                        {
                            Console.WriteLine("\n-----------------------------");
                            Console.Write("Opcija : ");
                            validno = int.TryParse(Console.ReadLine(), out opcija);
                        } while (opcija < 0 || opcija >= moguci_potezi.Count || !validno);

                        //slanje zeljene opcije
                        byte[] opcijaKojaSeSalje = Encoding.UTF8.GetBytes(opcija.ToString());
                        SocketUdp.SendTo(opcijaKojaSeSalje, ServerUdpEP);
                        Console.WriteLine("//////////////");
                    byte[] buffer1 = new byte[3048];
                    SocketUdp.Receive(buffer1);
                    string poruka1 = Encoding.UTF8.GetString(buffer1).Trim();
                    Console.WriteLine(poruka1);
                    }
                    else
                    {
                        Console.WriteLine(" Nema mogucih poteza.");
                        byte[] opcijaKojaSeSalje = Encoding.UTF8.GetBytes((-1).ToString());
                        SocketUdp.SendTo(opcijaKojaSeSalje, ServerUdpEP);
                        Console.WriteLine("//////////////");
                    }


                }
                else if (poruka.StartsWith("POBEDA|"))
                {
                    poruka = poruka.Substring("POBEDA|".Length);
                    Console.WriteLine(poruka);
                    break;
                }
                else
                {
                    Console.WriteLine(poruka);
                    continue;
                }
                //Console.ReadLine();

            } while (true);
        }
    }
}
