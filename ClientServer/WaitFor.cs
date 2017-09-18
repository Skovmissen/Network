using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkFramework;
using System.Threading;
namespace ClientServer
{
    class WaitFor
    {

        static List<Channel<string>> In = new List<Channel<string>>();
        static List<Channel<string>> Out = new List<Channel<string>>();
        static bool free = true;
        static bool denFemteVariable = true;
        static int req;
        static List<Channel<string>> chs = new List<Channel<string>>();
        static Channel<string> parent;
        static int EchoCount = 0;
        static bool IsInitializer = false;
        static SemaphoreSlim Sema;
        static SemaphoreSlim Ack;
        static Random rnd = new Random();

        public WaitFor()
        {
            Start();
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
                In.Add(ch);
                new Thread(() =>
                {
                    Console.WriteLine("new connection");
                    try
                    {
                        ThreadHandle(ch);
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
                        Out.Add(ch);
                        req++;

                        new Thread(() =>
                        {
                            try
                            {
                                ThreadHandle(ch);
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
                    else if (mess.StartsWith("!notify"))
                    {
                        IsInitializer = true;
                        if (denFemteVariable)
                        {
                            Notify(Out);
                        }
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
        //Garbage
        static void garbage(string msg, int actualWeight)
        {

            int k = rnd.Next(chs.Count);
            Shuffle(chs);
            int weight = getWeight(k, actualWeight);
            for (int i = 0; i < k; i++)
            {
                chs[i].Send(msg + weight);
                actualWeight -= weight;
            }
        }

        static int getWeight(int k, int actualWeight)
        {

            if ((actualWeight - k) < k)
            {
                actualWeight += Convert.ToInt32(Math.Pow(k, 2));
                if (!(IsInitializer))
                {
                    parent.Send("!garbageinit" + Convert.ToInt32(Math.Pow(k, 2)));
                }

            }
            return (actualWeight - k) / k;
        }
        static void recieve(string msg, int actualWeight, int startWeight, int weight)
        {
            if (IsInitializer && msg.StartsWith("!garbage"))
            {
                actualWeight += int.Parse(msg);
            }
            if (IsInitializer && actualWeight == startWeight)
            {
                Console.WriteLine("DONE!");
                
            }
            else if (msg.StartsWith("!garbage"))
            {

                parent.Send(msg);
            }
            else
            {
                actualWeight += weight;
                garbage(msg, actualWeight);
                parent.Send("!garbageinit" + actualWeight);
            }
        }
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        private static void Notify(List<Channel<string>> Out, Channel<string> ch)
        {
            Ack = new SemaphoreSlim(0);
            Sema = new SemaphoreSlim(0);
            denFemteVariable = false;
            string notifyMsg = "!notify";
            foreach (Channel<string> item in Out)
            {
                item.Send(notifyMsg);
            }



        }
        private static void Notify(List<Channel<string>> Out)
        {
            Ack = new SemaphoreSlim(0);
            Sema = new SemaphoreSlim(0);
            denFemteVariable = false;
            string notifyMsg = "!notify";
            foreach (Channel<string> item in Out)
            {
                item.Send(notifyMsg);
            }

        }
        private static void Grant(List<Channel<string>> In)
        {
            Ack = new SemaphoreSlim(0);
            if (free)
            {
                free = false;
                foreach (Channel<string> item in In)
                {
                    item.Send("!grant");
                }
                for (int i = 0; i < In.Count; i++)
                {
                    Ack.Wait();
                }

            }
        }
        private static void ThreadHandle(Channel<string> ch)
        {
            while (true)
            {
                string msg = ch.Receive();
                if (msg == null || msg == "")
                {
                    continue;
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
                else if (msg.StartsWith("!notify"))
                {
                    if (denFemteVariable)
                    {
                        Console.WriteLine("Notifying...");
                        Notify(Out, ch);


                        if (req == 0)
                        {
                            Grant(In);
                        }
                        for (int i = 0; i < Out.Count; i++)
                        {
                            Sema.Wait();
                        }


                    }
                    else
                    {
                        ch.Send("!done");

                    }
                }
                else if (msg == "!done")
                {
                    Sema.Release();
                }
                else if (msg == "!grant")
                {
                    if (req > 0)
                    {
                        req--;
                        if (req == 0)
                        {
                            Grant(In);
                        }
                    }
                    ch.Send("!ack");
                    Console.WriteLine("REQ!!!!");
                    Console.WriteLine(req);
                    Ack.Release();
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

