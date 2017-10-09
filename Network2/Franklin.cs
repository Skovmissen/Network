using NetworkFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        int recived = 2;
        Dictionary<int, List<int>> roundIds = new Dictionary<int, List<int>>();

        public Franklin(List<Channel<string>> chs)
        {
            _chs = chs;
            SendMsg(string.Format("!franklin-{0}-{1}", _id, _round));
        }

        public void Recieve(string msg, Channel<string> ch)
        {
            string[] buffer = msg.Split('-');
            int msgId = int.Parse(buffer[1]);
            int round = int.Parse(buffer[2]);

            if (round == _round)
                recived++;

            if (roundIds.ContainsKey(round))
                roundIds[round].Add(msgId);
            else
                roundIds.Add(round, new List<int>(msgId));

            if (_id == msgId)
            {
                _isLeader = true;
                Console.WriteLine("IM DA LEADER");
            }
            else if (!_isActive)
            {
                SendMsg(msg, ch);
            }
            else if (recived == 2)
            {
                _round++;
                recived = roundIds[_round].Count;
                DoRound();
            }
        }

        private void DoRound()
        {
            if (_id < roundIds[_round][0] || _id < roundIds[_round][1])
                _isActive = false;
            else
                SendMsg(string.Format("!franklin-{0}-{1}", _id, _round));
        }

        void SendMsg(string msg, Channel<string> noSend = null)
        {
            foreach (Channel<string> ch in _chs)
            {
                if (ch != noSend)
                    ch.Send(msg);
            }
        }
    }
}
