//using ServerCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;

////이렇게 세션을 나누는 이유는? 연결해야 하는게 더 있을 수 있다 (다른 서버, DB 등)
////이름이 클라이언트 세션인 이유는 클라이언트와 소통하니까

//namespace Server
//{
//    /*패킷을 구분하는 방법은 뭘까?
//ID로 1 이동 2 채팅 이런식으로 하는 방법이 있을 수 있다

//다만 문제는 경우에 따라 유동적으로 사이즈가 달라질 수 있다는 점이다
//그래서 첫 인자로 size 두번째로 ID를 넘겨주는 경우가 대다수이다. (int short 둘 중 ushort로 충분히 사용하긴 한다)*/
//    public abstract class Packet
//    {
//        //사이즈랑 아이디는 삭제해도 무관하다
//        public ushort size;
//        public ushort packetId;

//        //패킷 만드는거 자도오하 할거임
//        public abstract ArraySegment<byte> Write();
//        public abstract void Read(ArraySegment<byte> s);
//        //int로 하는것보다 4바이트를 아깔 수 있다.
//    }

///*    class LoginOKPacket : Packet
//    {
//        //이렇게 종속하게 만들어서 거의 제작한다.
//    }*/
//    //간단하게 패킷 구성(임시다 나중에는 자동화)
//    class PlayerInfoReq : Packet
//    {
//        public long playerId;

//        public PlayerInfoReq()
//        {
//            this.packetId = (ushort)PacketID.PlayerInfoReq;
//        }

//        public override void Read(ArraySegment<byte> s)
//        {
//            ushort count = 0;
//            //ushort size = BitConverter.ToUInt16(s.Array, s.Offset);//의미 없어보이긴함 여기있는거
//            count += 2;
//            //ushort id = BitConverter.ToUInt16(s.Array, s.Offset + count);//의미 없어보이긴함 여기있는거
//            count += 2;

//            this.playerId = BitConverter.ToInt64(new ReadOnlySpan<byte>(s.Array, s.Offset + count, s.Count - count));//이제 사이즈가 다르면 에러 뜬당 
//            //그런데 패킷을 신용할 수 없다고 했는데 왜 사이즈를 믿을까? 그건 반정도는 믿기 때문에 맞다고 가정하는 상황
//            count += 8;
//        }

//        public override ArraySegment<byte> Write()
//        {
//            ArraySegment<byte> s = SendBufferHelper.Open(4096);
//            bool success = true;
//            ushort count = 0;
//            // 유니티에서도 되는지는 확인해봐야 한다
//            count += 2; //걱정마 이것도 자동화 해줄거야
//            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.packetId); // 이제는 자신이 패킷
//            count += 2;
//            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.playerId);
//            count += 8;
//            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count);

//            if (!success) return null;
//            return SendBufferHelper.Close(12);
//        }
//    }

///*    class PlayerInfoOk : Packet
//    {
//        public int hp;
//        public int attack;
//    }*/

//    public enum PacketID // 지금은 이렇게 하드코딩 하지만 나중에는 자도오하
//    {
//        PlayerInfoReq = 1, PlayerInfoOk = 2,
//    }

//    class ClientSession : PacketSession //session > PacketSession
//    {
//        public override void OnConnected(EndPoint endPoint)
//        {
//            Console.WriteLine($"OnConnected {endPoint}");
//            //byte[] sendBuff = Encoding.UTF8.GetBytes("Welcom to MMORPG Server !");
//            //Packet packet = new Packet() { size = 100, packetId = 10};
//            //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
//            //byte[] buffer = BitConverter.GetBytes(packet.size);
//            //byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
//            //Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length); // buffer를 0에서부터 buffer길이만큼 sendbuff로 0에서부터 카피 
//            //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);// buffer2를 0에서부터 buffer2길이만큼 sendbuff로 buffer 길이 이후로 카피
//            //                                                                                              //이러면 만들어논 세그먼트에 데이터를 넣는다. 위와같이 하면 총 8byte를 사용했고 이를 닫아야한다.
//            //ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length); // 버퍼를 닫으면서 사용한 크기 전달
//            //                                                                                      //이때 sendBuff가 보내줘야하는 버퍼가 만들어진다.
//            //Send(sendBuff);
//            Thread.Sleep(5000); // 5초
//            Disconnect();
//        }

//        public override void OnDisconnected(EndPoint endPoint)
//        {
//            Console.WriteLine($"OnDisconnected {endPoint}");
//        }

//        public override void OnRecvPacket(ArraySegment<byte> buffer) // 유효한 범위를 보내주는거
//        {
//            //나중에는 자동화 해줄거니까 걱정하지 말자 그리고 이런 직렬화는 정말 많이 사용하니 잘 봐두자
//            ushort count = 0;
//            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
//            count += 2;
//            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
//            count += 2;

//            //이제 받는거니 파싱
//            //그런데 사이즈가 다른경우에는 실행이 안되도록 후에는 막아줘야 한다
//            switch((PacketID)id)
//            {
//                case PacketID.PlayerInfoReq:
//                    {
//                        /*long playerId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
//                        count += 8;*/
//                        PlayerInfoReq p = new PlayerInfoReq();
//                        p.Read(buffer);
//                        Console.WriteLine($"PlayerInfoReq : {p.playerId}");
//                    }
//                    break;
//            }
//            Console.WriteLine($"RecvPacket ID : {id}, size : {size}");
//        }

//        //sealed로 막아줘서 이제 사용 X
//        /*        public override int OnRecv(ArraySegment<byte> buffer) // 현재 하는 작업은 엔진과 컨텐츠를 분리하는 작업
//                {
//                    string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count); // 어디서부터 시작하냐 Offset
//                    Console.WriteLine($"[From client]{recvData}");
//                    return buffer.Count;
//                }*/


//        public override void OnSend(int numOfBytes)
//        {
//            Console.WriteLine($"Transferred byte {numOfBytes}");
//        }
//    }
//}
