/* Project name : ServerTchat
 * Description : Chat server application console for the project "Tchat"
 * Class : StateObject - State object for reading client data asynchronously
<<<<<<< HEAD
 * Original Author : https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example
 * Edited by : GENGA Dario
 * Last update : 2017.12.07 (yyyy-MM-dd)
 */
using System.Net.Sockets;
=======
 * Author : https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example
 * Last update : 2017.12.06 (yyyy-MM-dd)
 */
using System.Net.Sockets;
using System.Text;
>>>>>>> f6cedd8d5f5bd9a4b353cd9d75a96269024a19ea

namespace ServerTchat
{
    /// <summary>
    /// State object for reading client data asynchronously
    /// </summary>
    class StateObject
    {
<<<<<<< HEAD
        /// <summary>Client Socket.</summary> 
        public Socket WorkSocket = null;
        /// <summary>Receive buffer.</summary>
        public byte[] Buffer = new byte[1024];
=======
        // Client  socket.  
        public Socket WorkSocket = null;
        // Size of receive buffer.  
        public const int BUFFER_SIZE = 1024;
        // Receive buffer.  
        public byte[] Buffer = new byte[BUFFER_SIZE];
        // Received data string.  
        public StringBuilder Sb = new StringBuilder();
>>>>>>> f6cedd8d5f5bd9a4b353cd9d75a96269024a19ea
    }
}
