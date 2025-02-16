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
    public static void S_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastEnterGame Pkt = packet as S_BroadcastEnterGame;
        ServerSession serverSession = session as ServerSession;
        PlayerManager.Instance.EnterGame(Pkt);
    }

    public static void S_BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastLeaveGame Pkt = packet as S_BroadcastLeaveGame;
        ServerSession serverSession = session as ServerSession;
        PlayerManager.Instance.LeaveGame(Pkt);
    }

    public static void S_BroadcastMoveHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastMove Pkt = packet as S_BroadcastMove;
        ServerSession serverSession = session as ServerSession;

        PlayerManager.Instance.Move(Pkt);
    }

    public static void S_PlayerListHandler(PacketSession session, IPacket packet)
    {
        S_PlayerList Pkt = packet as S_PlayerList;
        ServerSession serverSession = session as ServerSession;
        PlayerManager.Instance.Add(Pkt);
    }
}