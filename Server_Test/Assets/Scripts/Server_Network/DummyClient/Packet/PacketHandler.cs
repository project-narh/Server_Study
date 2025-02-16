using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// 수동으로 관리하며 무엇을 호출할지
class PacketHandler
{

    public static void S_ChatHandler(PacketSession session, IPacket packet)
    {
        S_Chat chatPacket = packet as S_Chat;
        ServerSession serverSession = session as ServerSession;

        //if(chatPacket.playerId == 1)
        {
            Debug.Log(chatPacket.chat);

            //이게 왜 안되냐면 멀티 쓰레드 관련 문제 때문이야
            /*
            유니티를 조종하고있는 메인쓰레드에서 패킷을 관리하는게 아니라 쓰레드풀에서 가져와서 쓰는게 문제
            핸들러로 바로 처리하는게 아니라 큐에 넣고 유니티 쓰레드에서 처리하게 만들어주면 된다.
             */

            //이제 멀티쓰레드가 처리하는게 아니라 전송과정에서 유니티 큐로 보내고 NetworkManager에서 실행시켜주기에 유니티 메인쓰레드에서 실행된다
            //즉 유니티 작업이 가능해졌다
            GameObject go = GameObject.Find("Player");
            if (go == null) Debug.Log("Player not Found");
            else Debug.Log("Player Found");

        }


        //if (chatPacket.playerId == 1) // 10개 다 실행하면 로그가 너무 많아져서
        //Console.WriteLine(chatPacket.chat);

    }
}