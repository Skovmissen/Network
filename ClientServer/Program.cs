using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkFramework;
using System.Threading;

namespace ClientServer
{
    /// <summary>
    /// Nikolaj: 10.152.121.20
    /// Lasse: 10.152.121.21
    /// Kenneth: 10.152.121.22
    /// Bent: 10.152.121.23
    /// Anders: 10.152.120.34
    /// </summary>
    class Program
    {
        static List<Channel<string>> chs = new List<Channel<string>>();
        static Channel<string> parent;
        static int EchoCount = 0;
        static bool IsInitializer = false;

        static void Main(string[] args)
        
        {
            //WaitFor wf = new WaitFor();
            Garbage gc = new Garbage();
           
        }

      
    }
}
