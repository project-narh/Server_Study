using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class GameRoom // 세션은 사람이고 게임룸이 방이라고 생각하면 될거 같다
    {
        //List나 딕셔너리는 다중쓰레드를 고려하지 않는다 그래서 작업해줘야 함
        List<ClientSession> _sessions = new List<ClientSession>();
        object _lock = new object();

        public void Broadcast(ClientSession session, string chat)
        {
            S_Chat packet = new S_Chat();
            packet.playerId = session.SessionId; // 세션은 사람이고 게임룸이 방이라고 생각하면 될거 같다
            packet.chat = chat;
            ArraySegment<byte> segment = packet.Write();

            lock (_lock) // 공유하는 변수인 _sessions를 사용하니 락을 걸어야 한다
            {
                foreach(ClientSession s in _sessions)
                {
                    s.Send(segment);
                }
            }

        }

        public void Enter(ClientSession session) // 동시다발적으로 실행된다는걸 가정하고 해야한다
        {
            lock (_lock) // 한사람씩 들어오게
            {
                _sessions.Add(session);
                session.Room = this;
            }
        }

        public void Leave(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session);
            }
        }
    }
}
