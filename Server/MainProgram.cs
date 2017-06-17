using Server.database;
using Server.Singleton;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Entities;
using System.Text.RegularExpressions;

namespace Othello
{
    public class MainProgram
    {
        public SQLiteConnection connection;
        public static void Main()
        {
            
            Server _Server = new Server();

            _Server.StartServer();

            Console.ReadLine(); // When we press enter close everything
            Server.CloseAllSockets();
            



            //Console.WriteLine(hash);
           
            
            

        }

        
    }
}



