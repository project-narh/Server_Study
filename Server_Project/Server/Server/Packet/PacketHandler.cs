using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    // 수동으로 관리하며 무엇을 호출할지
    internal class PacketHandler
    {
        public static void PlayerInfoReqHandler(PacketSession session, IPacket packet)
        {
            PlayerInfoReq p = packet as PlayerInfoReq;
            Console.WriteLine($"PlayerInfoReq : {p.playerId} {p.name}");

            foreach (PlayerInfoReq.Skill skill in p.skills)
            {
                Console.WriteLine($"skill : {skill.id} {skill.level} {skill.duration}");
            }
        }
    }
}
