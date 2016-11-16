using Server.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiServer
{
    public class MainProgram
    {
        public static void Main()
        {
            Server _Server = new Server();

            _Server.StartServer();

            Console.ReadLine(); // When we press enter close everything
            Server.CloseAllSockets();

        }

       
    }
}
