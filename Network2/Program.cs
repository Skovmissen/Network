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
        static Garbage gar;
        static void Main(string[] args)
        {
            Start();
            //WaitFor t = new WaitFor();
        }

        static void Start()
        {
            TCPServer server = new TCPServer();

            new Thread(() =>
            {
                StartConsole();
            }).Start();

            while (true)
            {
                Channel<string> ch = server.Accept();
                chs.Add(ch);
                new Thread(() =>
                {
                    Console.WriteLine("new connection");
                    try
                    {
                        ConnectEcho(ch);
                        return;
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }).Start();
            }
        }

        static void StartConsole()
        {
            while (true)
            {
                try
                {
                    string mess = Console.ReadLine();
                    if (mess == "connect")
                    {
                        TCPClient ch = new TCPClient();
                        chs.Add(ch);

                        new Thread(() =>
                        {
                            try
                            {
                                ConnectEcho(ch);
                                return;
                            }
                            catch (Exception)
                            {
                                return;
                            }
                        }).Start();
                    }
                    else if (mess.StartsWith("!echo"))
                    {
                        IsInitializer = true;
                        SendMsg(mess);
                    }
                    else if (mess.StartsWith("!garbage"))
                    {
                        gar = new Garbage(chs, true);
                    }
                    else
                    {
                        SendMsg(mess);
                    }
                }
                catch (Exception)
                {
                    // do nothing
                }
            }
        }

        private static void ConnectEcho(Channel<string> ch)
        {
            while (true)
            {
                string msg = ch.Receive();
                if (msg == null || msg == "")
                {
                    continue;
                }
                else if (msg.StartsWith("!garbage"))
                {
                    if (gar == null)
                    {
                        gar = new Garbage(chs, false);
                        gar.Recieve(ch, msg);
                    }
                }
                else if (msg.StartsWith("!echo"))
                {
                    EchoCount++;
                    if (parent == null && !IsInitializer)
                    {
                        parent = ch;
                        parent.Send("Du er far til Skovmose");
                        SendEcho(msg);
                    }


                    if (EchoCount == chs.Count)
                    {
                        parent.Send(msg);
                    }
                }
                Console.WriteLine(msg);
            }
        }

        static void SendMsg(string msg)
        {
            foreach (Channel<string> ch in chs)
            {
                ch.Send(msg);
            }
        }
        static void SendEcho(string msg)
        {
            foreach (Channel<string> ch in chs)
            {
                if (parent != ch)
                {
                    ch.Send(msg);
                }
            }
        }
    }
}
