/* Project name : ServerTchat
 * Description : Chat server application console for the project "Tchat"
 * Class : StateObject - State object for reading client data asynchronously
 * Author : https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example
 * Last update : 2017.12.06 (yyyy-MM-dd)
 */
using System.Net.Sockets;
using System.Text;

namespace ServerTchat
{
    /// <summary>
    /// State object for reading client data asynchronously
    /// </summary>
    class StateObject
    {
        // Client  socket.  
        public Socket WorkSocket = null;
        // Size of receive buffer.  
        public const int BUFFER_SIZE = 1024;
        // Receive buffer.  
        public byte[] Buffer = new byte[BUFFER_SIZE];
        // Received data string.  
        public StringBuilder Sb = new StringBuilder();
    }
}
