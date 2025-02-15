using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient
{

    class ServerSession : Session
    {

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected {endPoint}");
      
            PlayerInfoReq packet = new PlayerInfoReq() { playerId = 1001, name = "ABCDEFG" };//이제 아이디 크기 필요없음

            packet.skills.Add(new PlayerInfoReq.Skill() { id = 101, level = 1, duration = 3.0f });
            packet.skills.Add(new PlayerInfoReq.Skill() { id = 102, level = 2, duration = 4.0f });
            packet.skills.Add(new PlayerInfoReq.Skill() { id = 103, level = 3, duration = 5.0f });
            packet.skills.Add(new PlayerInfoReq.Skill() { id = 104, level = 4, duration = 6.0f });
            //사실 보면 크기가 지금 아래를 보면 완전 달라진다 그래서 크기는 모든 크기를 계산하고 마지막에 넣는게 정석이다

            ArraySegment<byte> s = packet.Write();
            if (s != null) Send(s);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer) // 현재 하는 작업은 엔진과 컨텐츠를 분리하는 작업
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count); // 어디서부터 시작하냐 Offset
            Console.WriteLine($"[From Server]{recvData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred byte {numOfBytes}");
        }
    }
}
