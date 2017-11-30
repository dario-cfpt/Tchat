/* Project name : ServerTchat
 * Description : Chat server application console for the project "Tchat"
 * Author : GENGA Dario
 * Last update : 2017.11.30 (yyyy-MM-dd)
 */
using System;

namespace ServerTchat
{
    class Program
    {
        static void Main(string[] args)
        {
            // Start the server
            Server server = new Server();
            Console.ReadLine();
        }
    }
}
