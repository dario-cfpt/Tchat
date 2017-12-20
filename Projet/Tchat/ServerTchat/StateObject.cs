/* Project name : ServerTchat
 * Description : Chat server application console for the project "Tchat"
 * Class : StateObject - State object for reading client data asynchronously
 * Original Author : https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example
 * Edited by : GENGA Dario
 * Last update : 2017.12.17 (yyyy-MM-dd)
 */
using System.Net.Sockets;

namespace ServerTchat
{
    /// <summary>
    /// State object for reading client data asynchronously
    /// </summary>
    class StateObject
    {
        /// <summary>Client Socket</summary> 
        public Socket WorkSocket = null;
        /// <summary>Receive buffer</summary>
        public byte[] Buffer = new byte[1048576]; // maximal data that we can receive by the client
    }
}
