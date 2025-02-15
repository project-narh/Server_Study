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
    public static void C_ChatHandler(PacketSession session, IPacket packet)
    {
        C_Chat chatPacket = packet as C_Chat;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        //clientSession.Room.Push(
        //    () => clientSession.Room.Broadcast(clientSession, chatPacket.chat)
        //    );

        GameRoom room = clientSession.Room;
        room.Push(
            () => room.Broadcast(clientSession, chatPacket.chat)
            );

        // 원래는 바로 룸에 접근해서 했지만 해야하는 행동을 보낸 차이
        // 이렇게 하면 문제 생길 수 있다 예를들어 Room에 null이 되면.. 실제로돌려보니 에러뜬다
        // 비동기로 실행되기 때문에 clientSession.Room이 null이 되면 Use-After-Free 오류(이미 해제된 객체 접근)
        //그래서 다른곳에 미리 저장해두고 나가서 이전에 사라진 참조를 접근하는게 아니라서 안전하다
        //이를 떠날때도 해줘야해 강제종료 될 수 있잖아
    }
}

