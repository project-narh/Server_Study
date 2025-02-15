using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
    #region Singleton (이전 방식은 매번 실행될때 래지스터 호출해줘야 해서 그런 작업 안하게 수정)
    static PacketManager _instance = new PacketManager();
    public static PacketManager Instance { get { return _instance; } }
    #endregion
    
    PacketManager() 
    {
        Register();
    }
    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();//프로토콜ID, 행동
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

    public void Register()
    {
        //패킷을 받는도중 Register를 하면 문제 발생 (먼저 등록하고 패킷이 들어오면 문제가 안된다)

        _onRecv.Add((ushort)PacketID.S_Chat, MakePacket<S_Chat>);
        _handler.Add((ushort)PacketID.S_Chat, PacketHandler.S_ChatHandler);

    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)// 유효한 범위를 보내주는거
    {
        //나중에는 자동화 해줄거니까 걱정하지 말자 그리고 이런 직렬화는 정말 많이 사용하니 잘 봐두자
        //이제 매니저로 이동(자동화 위해서)
        ushort count = 0;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        /*//이제 받는거니 파싱
        //그런데 사이즈가 다른경우에는 실행이 안되도록 후에는 막아줘야 한다
        //switch로 하면 자동화가 아니다 이제 이거 바꿔줄거야
        //switch ((PacketID)id)
        //{
        //    case PacketID.PlayerInfoReq:
        //        {
        //            //long playerId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        //            //count += 8;
        //            PlayerInfoReq p = new PlayerInfoReq();
        //            p.Read(buffer);

        //        }
        //        break;
        //아래 코드로 변경
        //}*/

        Action<PacketSession, ArraySegment<byte>> action = null;
        if(_onRecv.TryGetValue(id, out action))
            action.Invoke(session, buffer);
        //이게 실행되고 위에 딕셔너리로 등록한 핸들러에 접근되고 그 핸들러가 MakePacket 메소드를 실행한다

    }

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        /*long playerId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
                    count += 8;*/
        T pkt = new T();
        pkt.Read(buffer);

        Action<PacketSession, IPacket> action = null;
        if(_handler.TryGetValue(pkt.Protocol, out action))
            action.Invoke(session, pkt);
    }
}
