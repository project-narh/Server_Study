using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

//이렇게 세션을 나누는 이유는? 연결해야 하는게 더 있을 수 있다 (다른 서버, DB 등)
//이름이 클라이언트 세션인 이유는 클라이언트와 소통하니까

namespace Server
{
    class ClientSession : PacketSession //session > PacketSession
    {
        public int SessionId { get; set; }
        public GameRoom Room { get; set; }
        //원래는 플레이어 클래스를 만들어야겠지만 간단하게 여기다 구현

        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected {endPoint}");
            Program.Room.Push(() => Program.Room.Enter(this));
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            SessionManager.Instance.Remove(this);
            if(Room != null)
            {
                GameRoom room = Room;
                room.Push(() => room.Leave(this));
                Room = null;
            }

            Console.WriteLine($"OnDisconnected {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer) // 유효한 범위를 보내주는거
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }



        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Transferred byte {numOfBytes}");
        }
    }
}
