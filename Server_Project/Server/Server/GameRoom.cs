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

        public void Broadcast(ArraySegment<byte> segment)
        {
            _pendingList.Add(segment);
        }

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
            //플레이어 추가
            _sessions.Add(session);
            session.Room = this;

            //신입생한테 모든 플레이어 목록 전송
            S_PlayerList players = new S_PlayerList();
            foreach(ClientSession s in _sessions)
            {
                players.players.Add(new S_PlayerList.Player()
                {
                    isSelf = (s == session),
                    playerId = s.SessionId,
                    posX = s.PosX,
                    posY = s.PosY,
                    posZ = s.PosZ
                });
            }
            session.Send(players.Write());

            //신입생 입장을 모두에게 알린다
            S_BroadcastEnterGame enter = new S_BroadcastEnterGame();
            enter.playerId = session.SessionId;
            enter.posX = 0;
            enter.posY = 0;
            enter.posZ = 0;
            Broadcast(enter.Write());
        }

        public void Leave(ClientSession session)
        {
            // 플레이어 제거
            _sessions.Remove(session);  

            // 모두에게 알린다
            S_BroadcastLeaveGame leave = new S_BroadcastLeaveGame();
            leave.playerId = session.SessionId;
            Broadcast(leave.Write());
        }

        public void Move(ClientSession session, C_Move packet)
        {
            //좌표를 바꿔주고
            session.PosX = packet.posX;
            session.PosY = packet.posY;
            session.PosZ = packet.posZ;

            //모두에게 알린다
            S_BroadcastMove move = new S_BroadcastMove();
            move.playerId = session.SessionId;
            move.posX = session.PosX;
            move.posY = session.PosY;
            move.posZ = session.PosZ;

        }
    }
}
