using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

//이렇게 세션을 나누는 이유는? 연결해야 하는게 더 있을 수 있다 (다른 서버, DB 등)
//이름이 클라이언트 세션인 이유는 클라이언트와 소통하니까

namespace Server
{
    /*패킷을 구분하는 방법은 뭘까?
ID로 1 이동 2 채팅 이런식으로 하는 방법이 있을 수 있다

다만 문제는 경우에 따라 유동적으로 사이즈가 달라질 수 있다는 점이다
그래서 첫 인자로 size 두번째로 ID를 넘겨주는 경우가 대다수이다. (int short 둘 중 ushort로 충분히 사용하긴 한다)*/
    public abstract class Packet
    {
        //사이즈랑 아이디는 삭제해도 무관하다
        public ushort size;
        public ushort packetId;

        //패킷 만드는거 자도오하 할거임
        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> s);
        //int로 하는것보다 4바이트를 아깔 수 있다.
    }

/*    class LoginOKPacket : Packet
    {
        //이렇게 종속하게 만들어서 거의 제작한다.
    }*/
    //간단하게 패킷 구성(임시다 나중에는 자동화)
    class PlayerInfoReq : Packet
    {
        public long playerId;
        public string name;

        public PlayerInfoReq()
        {
            this.packetId = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> segment)
        {
            //넘겨준 사이즈와 관계없이 현재 비트 컴버터로 가져오는 크기를 강제하고 있기에 사이즈가 달라도 실행되고 있다
            ushort count = 0;
            ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
            count += sizeof(long);

            //string
            ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);

            this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));



        }
        //Write 부분은 직접 컨트롤 하고 있는걸 상관없는데 Read는 악의적인 패킷에 위험하다 (항상 클라는 거짓말은 한다고 가정해서 만들어야 한다)
        //패킷 해더는 참고용이지 믿으면 안된다
        public override ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);//이건 이렇게도 할 수 있다 해서 보여주는거 만들어둔거 슬라이스로 자르는 방식
            bool success = true;
            ushort count = 0;

            // 유니티에서도 되는지는 확인해봐야 한다
            count += sizeof(ushort);
            //span 범위를 잡아준다
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.packetId); // 이제는 자신이 패킷
            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
            count += sizeof(long); ;

            //여기에서 String 관련 처리 해줘야 할텐데 string은 utf16
            //string len[2] 2byte로 스트링의 길이를 알려주고
            //byte[] 해당하는 데이터를 같이 보낸다
            //문제는 문자열은 length를 쓰면 길이가 나오지만 바이트로 바꾸고 길이를 재면 2배가 된다 왜냐면 한칸당 2byte니까
            //이는 전에 패킷쓸때 헤더 넣는거랑 동일
            ushort nameLen = (ushort)Encoding.Unicode.GetByteCount(this.name);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
            count += sizeof(ushort);
            Array.Copy(Encoding.Unicode.GetBytes(this.name), 0, segment.Array, count, nameLen);
            count += nameLen;
            success &= BitConverter.TryWriteBytes(s, count);






            //// 유니티에서도 되는지는 확인해봐야 한다
            //count += sizeof(ushort);
            ////span 범위를 잡아준다
            //success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.packetId); // 이제는 자신이 패킷
            //count += sizeof(ushort);
            //success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.playerId);
            //count += sizeof(long); ;
            //success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count);

            if (!success) return null;
            return SendBufferHelper.Close(count);
        }
    }

    /*    class PlayerInfoOk : Packet
        {
            public int hp;
            public int attack;
        }*/

    public enum PacketID // 지금은 이렇게 하드코딩 하지만 나중에는 자도오하
    {
        PlayerInfoReq = 1, PlayerInfoOk = 2,
    }

    class ClientSession : PacketSession //session > PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected {endPoint}");
            //byte[] sendBuff = Encoding.UTF8.GetBytes("Welcom to MMORPG Server !");
            //Packet packet = new Packet() { size = 100, packetId = 10};
            //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            //byte[] buffer = BitConverter.GetBytes(packet.size);
            //byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
            //Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length); // buffer를 0에서부터 buffer길이만큼 sendbuff로 0에서부터 카피 
            //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);// buffer2를 0에서부터 buffer2길이만큼 sendbuff로 buffer 길이 이후로 카피
            //                                                                                              //이러면 만들어논 세그먼트에 데이터를 넣는다. 위와같이 하면 총 8byte를 사용했고 이를 닫아야한다.
            //ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length); // 버퍼를 닫으면서 사용한 크기 전달
            //                                                                                      //이때 sendBuff가 보내줘야하는 버퍼가 만들어진다.
            //Send(sendBuff);
            Thread.Sleep(5000); // 5초
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer) // 유효한 범위를 보내주는거
        {
            //나중에는 자동화 해줄거니까 걱정하지 말자 그리고 이런 직렬화는 정말 많이 사용하니 잘 봐두자
            ushort count = 0;
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += 2;
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;

            //이제 받는거니 파싱
            //그런데 사이즈가 다른경우에는 실행이 안되도록 후에는 막아줘야 한다
            switch((PacketID)id)
            {
                case PacketID.PlayerInfoReq:
                    {
                        /*long playerId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
                        count += 8;*/
                        PlayerInfoReq p = new PlayerInfoReq();
                        p.Read(buffer);
                        Console.WriteLine($"PlayerInfoReq : {p.playerId} {p.name}");
                    }
                    break;
            }
            Console.WriteLine($"RecvPacket ID : {id}, size : {size}");
        }

        //sealed로 막아줘서 이제 사용 X
        /*        public override int OnRecv(ArraySegment<byte> buffer) // 현재 하는 작업은 엔진과 컨텐츠를 분리하는 작업
                {
                    string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count); // 어디서부터 시작하냐 Offset
                    Console.WriteLine($"[From client]{recvData}");
                    return buffer.Count;
                }*/


        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred byte {numOfBytes}");
        }
    }
}
