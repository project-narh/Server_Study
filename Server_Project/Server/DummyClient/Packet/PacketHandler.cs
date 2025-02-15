using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 수동으로 관리하며 무엇을 호출할지
class PacketHandler
{

    public static void S_ChatHandler(PacketSession session, IPacket packet)
    {
        S_Chat chatPacket = packet as S_Chat;
        ServerSession serverSession = session as ServerSession;

        //if (chatPacket.playerId == 1) // 10개 다 실행하면 로그가 너무 많아져서
            Console.WriteLine(chatPacket.chat);

    }
}