using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BatExtension
{
    public class BatListener
    {
        private TcpListener tcpListener;
        public bool Running = false;
        private int count;

        // Event to notify when a new line is received
        public event Action<string>? LineReceived;

        public BatListener()
        {
            // Initialize the TCP listener on port 5000
            tcpListener = new TcpListener(IPAddress.Any, 5000);
        }

        public void Listen()
        {
            try
            {
                tcpListener.Start();
                Console.WriteLine("Server started, waiting for connections...");

                while (Running)
                {
                    using (var client = tcpListener.AcceptTcpClient())
                    {
                        using (var stream = client.GetStream())
                        using (var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true))
                        {
                            string? line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (!string.IsNullOrWhiteSpace(line))
                                {
                                    Console.WriteLine(line);
                                    LineReceived?.Invoke(line); // Trigger the event
                                }
                            }
                        }
                        var testi = "Got data! " + count;
                        Console.WriteLine(testi); // Log the count
                        count++;
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"An IO exception occurred: {ex.Message}");
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine($"The socket was closed: {ex.Message}");
            }
            finally
            {
                tcpListener.Stop();
            }
        }
    }
}