using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

            EndPoint ServerUdpEP;

            byte[] buffer = new byte[2048];

            try
            {
                clientSocket.Connect(serverEP);
                Console.WriteLine("Korisnik je uspesno povezan sa serverom!");
            }

            catch(SocketException ex)
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

                SocketUdp.ReceiveFrom(buffer, ref ServerUdpEP);
                Console.WriteLine(Encoding.UTF8.GetString(buffer));
                Console.ReadLine();
                
            }

            catch (Exception ex) 
            {
                Console.WriteLine($"Doslo je do greske {ex.Message}!");
            }
        }
    }
}
