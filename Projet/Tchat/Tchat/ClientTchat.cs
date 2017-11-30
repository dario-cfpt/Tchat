using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Tchat
{
    class ClientTchat
    {
        private TcpClient _client;
        

        public ClientTchat(IPAddress address, int port)
        {
            IPEndPoint ipEndPoint = new IPEndPoint(address, port);
            StartConnection(ipEndPoint);
        }

        private void StartConnection(IPEndPoint ipEndPoint)
        {            
            _client = new TcpClient(ipEndPoint);

            NetworkStream ns = _client.GetStream();
        }

    }
}
