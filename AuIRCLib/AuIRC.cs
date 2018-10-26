using System;
using System.IO;
using System.Net.Sockets;


namespace AuIRCLib
{
    public class AuIRC
    {
        private TcpClient client;
        private StreamReader sr;
        private StreamWriter sw;
        private bool isConnected = false;

        private string host;
        private ushort port;

        private string nick, user, channel;

        public bool Connect(string host, ushort port = 6667, uint timeout = 5)
        {
            bool success = false;
            try
            {
                if (Connected())
                    throw new Exception("Connection already exists");
                client = new TcpClient();
                IAsyncResult result = client.BeginConnect(host, port, null, null);

                if (!result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(timeout)))
                    throw new Exception($"Failed to connect to {host}:{port}");

                client.EndConnect(result);

                sr = new StreamReader(client.GetStream());
                sw = new StreamWriter(client.GetStream());
                success = true;
                this.host = host;
                this.port = port;

                Console.WriteLine($"Successfully connected to {host}:{port}");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Source + ": " + e.Message);
            }

            isConnected = success;
            return success;
        }

        public void Disconnect()
        {
            try
            {
                if (Connected())
                {
                    client.GetStream().Close();
                    client.Close();
                    client = null;
                    sr.Close();
                    sr = null;
                    sw.Close();
                    sw = null;
                    host = null;
                    port = 0;
                    isConnected = false;
                }
                else
                    throw new Exception("Not connected");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Source + ": " + e.Message);
            }
        }

        public bool Connected()
        {
            return isConnected;
        }

        public void Send(string message)
        {
            try
            {
                if (Connected())
                {
                    sw.WriteLine(message);
                    sw.Flush();
                }
                else
                    throw new Exception("Not connected");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Source + ": " + e.Message);
            }
        }

        public string Receive()
        {
            string result = "";

            try
            {
                if(Connected())
                {
                    result = sr.ReadLine();
                }
                else
                    throw new Exception("Not connected");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Source + ": " + e.Message);
            }

            return result;
        }

        public void JoinChannel(string channel)
        {
            Send($"JOIN {channel}");
            this.channel = channel;
        }

        public void SetNick(string nick)
        {
            Send($"NICK {nick}");
            this.nick = nick;
        }

        public void SetUser(string user)
        {
            Send($"USER {user} 0 * :{user}");
            this.user = user; 
        }
    }
}
