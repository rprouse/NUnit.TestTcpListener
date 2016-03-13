using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Mono.Options;

namespace NUnit.TestTcpListener
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 13000.
                int port = 13000;
                var localAddr = IPAddress.Parse("127.0.0.1");
                string outputPath = @".\output.xml";

                var options = new OptionSet
                                  {
                                      {
                                          "h|hostname=", "the host dns name or IP address.",
                                          host => localAddr = IPAddress.Parse(host)
                                      },
                                      {
                                          "p|port=", "the port number to listen to.\n" + "this must be an integer.",
                                          (int p) => port = p
                                      },
                                      {
                                          "o|output=", "the path of the output result.",
                                          o => outputPath = o
                                      }
                                  };

                options.Parse(args);

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                var bytes = new byte[256];

                // Enter the listening loop.
                //while (true)
                {
                    Console.Write($"Waiting for a connection on {localAddr}:{port}, will drop output to {outputPath}");

                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    StringBuilder builder = new StringBuilder();

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        var data = Encoding.UTF8.GetString(bytes, 0, i);
                        builder.Append(data);
                        Console.WriteLine("Received: {0}", data);
                    }

                    // Shutdown and end connection
                    client.Close();

                    // Create a file to write to.                        
                    File.WriteAllText(outputPath, builder.ToString());
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server?.Stop();
            }
        }
    }
}
