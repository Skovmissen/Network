using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkFramework;

namespace ClientServer
{
    class Garbage
    {
        static Random rnd = new Random();
        int _actualWeight;
        int _startWeight;
        bool _isinit;
        Channel<string> parent;
        List<Channel<string>> chs;

        public Garbage(List<Channel<string>> _chs, bool isInit)
        {
            chs = _chs;
            _startWeight = 10 * chs.Count;
            _actualWeight = _startWeight;
            _isinit = isInit;

            if (_isinit)
            {
                Console.WriteLine("starting garbage algo...");
                Algo();
            }
        }

        void Algo()
        {
            Console.WriteLine("chs count: " + chs.Count);
            int k = rnd.Next(chs.Count);


            Shuffle(chs);

            int weight = getWeight(k);
            for (int i = 0; i < k; i++)
            {
                chs[i].Send("!garbage-" + weight);
                _actualWeight -= weight;
            }
        }

        public void Recieve(Channel<string> sender, string msg)
        {

            if (!_isinit && parent == null)
            {
                Console.WriteLine("setting parent...");
                parent = sender;
            }
            if (_isinit && msg.StartsWith("!garbageinit"))
            {
                int weight = int.Parse(msg.Split('-')[1]);
                _actualWeight += weight;

                if (_actualWeight == _startWeight)
                {
                    Console.WriteLine("Lasse er ikke awesome");
                    return;
                }
            }
            else if (_isinit && msg.StartsWith("!garbageStartWeightinit"))
            {
                Console.WriteLine("updating start weight... ");
                int weight = int.Parse(msg.Split('-')[1]);
                Console.WriteLine("adding " +  weight + " to start weight");
                _startWeight += weight;
            }
            else if (msg.StartsWith("!garbageinit") || msg.StartsWith("!garbageStartWeightinit"))
            {
                parent.Send(msg);
            }
            else
            {
                int weight = int.Parse(msg.Split('-')[1]);
                _actualWeight += weight;
                Algo();
                parent.Send("!garbageinit-" + _actualWeight);
                _actualWeight = 0;
            }
        }

        public static void Shuffle(List<Channel<string>> chs)
        {
            int n = chs.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                Channel<string> value = chs[k];
                chs[k] = chs[n];
                chs[n] = value;
            }
        }

        int getWeight(int k)
        {

            if ((_startWeight - k) < k)
            {
                _actualWeight += Convert.ToInt32(Math.Pow(k, 2));
                if (!(_isinit))
                {
                    parent.Send("!garbageStartWeightinit-" + Convert.ToInt32(Math.Pow(k, 2)));
                }

            }
            return (_actualWeight - k) / k;
        }
    }
}