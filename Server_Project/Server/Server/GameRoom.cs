using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    //이번에는 컨텐츠에서 패킷모아보내기 진행 (엔진에서 하는건 서버코어-세션-send를 바로 보내는게 아니라 어느정도 모와서 보내면 된다)
    internal class GameRoom : IJobQueue
    {
        // 세션은 사람이고 게임룸이 방이라고 생각하면 될거 같다
        //List나 딕셔너리는 다중쓰레드를 고려하지 않는다 그래서 작업해줘야 함
        List<ClientSession> _sessions = new List<ClientSession>();
        object _lock = new object();
        JobQueue _jobQueue = new JobQueue(); // 행동들을 job으로 넣는 방식으로
        //패킷 모아보내기 위한 리스트
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        //누가 잡을 가지고 있어야 할까?
        //씬 단위가 있으면 씬마다 배치 리니지나 와우같이 씬리스인 경우는 모든 사물에 넣어야 한다(존단위로 하면 애매함)

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        public void Broadcast(ClientSession session, string chat)
        {
            S_Chat packet = new S_Chat();
            packet.playerId = session.SessionId; // 세션은 사람이고 게임룸이 방이라고 생각하면 될거 같다
            packet.chat = $"{chat} I am {packet.playerId}";
            ArraySegment<byte> segment = packet.Write();

            //lock을 잡고 디버그해보면 대부분의 쓰레드가 이쪽에 잡혀있다 
            //이곳에서 밀리면 쓰레드 회수가 안되고 풀입장에서는 너무 늦어서 새로 쓰레드를 생성한다. 무식하게 직원을 100명씩 고용하는 상황
            //잘 모르는 사람들은 락잡으면 끝이라고 생각하지만 일은 한번에 한명씩밖에 못한다 진지하게 하려면 이렇게 하면 안된다.
            //소규모면 몰라도 대규모면 이렇게 하면 밀린다.
            // 방법은 게임룸 작업을 한명한 하게 한다 나머지 애들은 큐에 넣고 다른일을 한다.

            //MMO에서 서버가 어떻게 구성되나 중요하다.
            //N제곱의 시간복잡도(주변 모두에게 보내기 때문에)
            //주변에 100명 있으면 100명 모두에게 보내기 때문에 그래서 테스트 해보면 같은 숫자가 10개씩 한번에 보내짐
            //이 보내는 공간 잡는게 힘들다...

            //foreach(ClientSession s in _sessions)
            //{
            //    s.Send(segment);
            //}

            _pendingList.Add(segment); // 위처럼 각각 다 보내는게 아니라 보내야 할 패킷을 모은다.
            //그러면 모은경우 누군가는 보내긴 해야 한다.

        }


        //클라이언트 요청만이 아닌 몬스터가 움직이거나 길찾기, 스킬 이런것들도 게임룸에 넣어야 한다
        //실행하기 위한 마스터가 필요하다
        //Main에서 클라이언트의 요청을 담은 Room을 Flush하는 역할을 했는데 서버도 마찬가지로 Flush해 갱신해주는 작업이 필요하다.
        //유저들이 보낸 패킷 뿐만 아니라 AI등의 작업도 잡큐 _pendingList에 넣어야 한다.
        public void Flush()
        {
            foreach (ClientSession s in _sessions)
            {
                s.Send(_pendingList);
            }
            Console.WriteLine($"Flushed {_pendingList.Count} items");
            _pendingList.Clear(); // 보냈으니 정리
        }

        public void Enter(ClientSession session) // 동시다발적으로 실행된다는걸 가정하고 해야한다
        {
            _sessions.Add(session);
            session.Room = this;
        }

        public void Leave(ClientSession session)
        {
            _sessions.Remove(session);  
        }
    }
}
