using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace DummyClient
{

    class ServerSession : PacketSession
    {

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected {endPoint}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer) // 현재 하는 작업은 엔진과 컨텐츠를 분리하는 작업
        {
            //패킷을 큐에 넣어준다(유니티 쓰레드에서 처리할 수 있도록)
            PacketManager.Instance.OnRecvPacket(this, buffer, (s,p) => PacketQueue.Instance.Push(p));
        }

        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Transferred byte {numOfBytes}");
            Debug.Log("보내는데 성공");
        }
    }
}
