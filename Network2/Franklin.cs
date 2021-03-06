﻿using NetworkFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientServer
{
    class Franklin
    {
        List<Channel<string>> _chs;
        bool _isActive = true;
        bool _isLeader = false;
        int _id = 10;
        int _round = 1;
        int recived = 0;
        int neighone = -1;
        int neightwo = -1;

        SemaphoreSlim sema = new SemaphoreSlim(1); 

        public Franklin(List<Channel<string>> chs)
        {
            Console.WriteLine("new franklin");
            _chs = chs;
            SendMsg(string.Format("!franklin-{0}-{1}", _id, _round));
        }

        public void Recieve(string msg, Channel<string> ch)
        {
            sema.Wait();
            Console.WriteLine("recived: " + msg);
            string[] buffer = msg.Split('-');
            int msgId = int.Parse(buffer[1]);
            int round = int.Parse(buffer[2]);

            Console.WriteLine("msgid: " + msgId);
            Console.WriteLine("round: " + round);

            if (round == _round)
            {
                recived++;

                if (recived == 1)
                    neighone = msgId;
                else
                    neightwo = msgId;
            }



            if (_id == msgId)
            {
                _isLeader = true;
                Console.WriteLine("IM DA LEADER");
            }
            else if (!_isActive)
            {
                Console.WriteLine("parsing msg...");
                SendMsg(msg, ch);
            }
            else if (recived == 2)
            {
                Console.WriteLine("recived 2 in round " + _round + " sending...");
                _round++;
                recived = 0;
                Console.WriteLine("ROUND DEBUG: " + _round);
                recived = 0;
                Console.WriteLine("RECEIVED DEBUG: " + recived);
                DoRound();
            }
            sema.Release();
        }

        private void DoRound()
        {
            Console.WriteLine("doround();");
            if (_id <neighone || _id < neightwo)
            {
                _isActive = false;
                Console.WriteLine("setting myself to not active...");
            }
            else
            {
                Console.WriteLine("sending new round - im the biggest");
                SendMsg(string.Format("!franklin-{0}-{1}", _id, _round));
            }
        }

        void SendMsg(string msg, Channel<string> noSend = null)
        {
            Console.WriteLine("sending: " + msg);
            foreach (Channel<string> ch in _chs)
            {
                if (ch != noSend)
                    ch.Send(msg);
            }
        }
    }
}
