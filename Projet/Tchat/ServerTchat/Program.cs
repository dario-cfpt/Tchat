/* Project name : ServerTchat
 * Description : Chat server application console for the project "Tchat"
 * Class : Program - Start the server
 * Author : GENGA Dario
 * Last update : 2017.12.07 (yyyy-MM-dd)
 */
using System;

namespace ServerTchat
{
    /// <summary>
    /// Start the server
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            Console.ReadLine();
        }
    }
}
