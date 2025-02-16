using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 수동으로 관리하며 무엇을 호출할지
class PacketHandler
{
    public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.Leave(clientSession));
    }

    public static void C_MoveHandler(PacketSession session, IPacket packet)
    {
        C_Move movePacket = packet as C_Move;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        //Console.WriteLine($"{movePacket.posX}, {movePacket.posY}, {movePacket.posZ}");

        GameRoom room = clientSession.Room;
        room.Push(() => room.Move(clientSession, movePacket));
    }
}

