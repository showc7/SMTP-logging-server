using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace SMTP
{
    class Program
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 25);
            _logger.Info("Server started on " + endPoint.Address + ":" + endPoint.Port);
            TcpListener listener = new TcpListener(endPoint);
            listener.Start();

            while(true)
            {
                TcpClient client = listener.AcceptTcpClient();
                SMTPServer handler = new SMTPServer();
                //servers.Add(handler);
                handler.Init(client);
                Thread thread = new Thread(new ThreadStart(handler.Run));
                thread.Start();
            }
        }
    }
}
