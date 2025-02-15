﻿using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 수동으로 관리하며 무엇을 호출할지
class PacketHandler
{
    public static void C_PlayerInfoReqHandler(PacketSession session, IPacket packet)
    {
        C_PlayerInfoReq p = packet as C_PlayerInfoReq;
        Console.WriteLine($"PlayerInfoReq : {p.playerId} {p.name}");

        foreach (C_PlayerInfoReq.Skill skill in p.skills)
        {
            Console.WriteLine($"skill : {skill.id} {skill.level} {skill.duration}");
        }
    }
}

