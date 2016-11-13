using System;
using System.Net.Sockets;
using System.Text;
using NLog;

namespace SMTP
{
    internal class SMTPServer
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        private TcpClient client;
        public SMTPServer()
        {
        }

        public void Init(TcpClient client)
        {
            this.client = client;
        }

        public void Run()
        {
            _logger.Info("client connected " + client.Client.RemoteEndPoint);

            while(true)
            {
                if(!AcceptMessage())
                {
                    break;
                }
            }
        }

        private bool AcceptMessage()
        {
            string message = string.Empty;
            try
            {
                message = Read();
                _logger.Info("client message: {" + message + "}");
            }
            catch(Exception e)
            {
                _logger.Error(e.Message + "\n" + e.Source + "\n" + e.StackTrace);
                return false;
            }

            if(message.Length > 0)
            {
                if(message.StartsWith("QUIT"))
                {
                    client.Close();
                    return false;
                }
                if(message.StartsWith("EHLO"))
                {
                    Write("250 OK");
                }
                if(message.StartsWith("RCPT TO"))
                {
                    Write("250 OK");
                }
                if(message.StartsWith("MAIL FROM"))
                {
                    Write("250 OK");
                }
                if(message.StartsWith("DATA"))
                {
                    Write("354 Start mail input; end with");
                    message = Read();
                    Write("250 OK");
                }
            }

            return true;
        }

        private string Read()
        {
            int bufferSize = 8192;
            byte[] messageBytes = new byte[bufferSize];
            NetworkStream clientStream = client.GetStream();
            ASCIIEncoding encoder = new ASCIIEncoding();
            int bytesRead = clientStream.Read(messageBytes, 0, bufferSize);
            return encoder.GetString(messageBytes,0,bytesRead);
        }

        private void Write(string message)
        {
            NetworkStream clientStream = client.GetStream();
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] buffer = encoder.GetBytes(message + "\r\n");
            clientStream.Write(buffer,0,buffer.Length);
            clientStream.Flush();
        }
    }
}