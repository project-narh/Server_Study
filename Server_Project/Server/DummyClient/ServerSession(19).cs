using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient
{
    //클라이언트에서 대리자가 서버 세션
    //서버쪽의 대리자가 클라이언트 세션
    class Packet
    {
        public ushort size;
        public ushort packetId;
        //int로 하는것보다 4바이트를 아깔 수 있다.
    }
    //메모리상에 인스턴스로 존제하는 데이터를 납작하게 눌러서 버퍼에 넣는 작업 = 직렬화 
    //BYTE -> 인스턴스 역직렬화
    //일반적으로 세이브 파일을 만든다고 하면 파일로 만들어서 저장할거다 이도 바이트로 만들어서 보낼텐데 이것도 직렬화

    //간단하게 패킷 구성(임시다 나중에는 자동화) 패킷을 관리하는 방법 다 다르다 json으로 관리하는 경우도 많다
    class PlayerInfoReq : Packet
    {
        public long playerId;
    }

    class PlayerInfoOk : Packet
    {
        public int hp;
        public int attack;
    }

    public enum PacketID // 지금은 이렇게 하드코딩 하지만 나중에는 자도오하
    {
        PlayerInfoReq = 1, PlayerInfoOk = 2,
    }

    class ServerSession : Session
    {
/*        static unsafe void ToBytes(byte[] array, int offset, ulong value) // unsafe를 사용하면 C++마냥 포인터를 사용할 수 있게 된다
        {
            fixed (byte* ptr = &array[offset]) // 객체 포인터를 고정시켜서 가비지 컬렉터가 못가져가게
                *(ulong*)ptr = value;
        }//사용하는건 아니고 예시를 보여준거야*/

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected {endPoint}");
            //PlayerInfoReq packet = new PlayerInfoReq() { size = 4, packetId = 7 }; // 서버에 붙자마자 OnConnected가 실행되면 플레이어 정보를 받는다
            PlayerInfoReq packet = new PlayerInfoReq() { size = 4, packetId = (ushort) PacketID.PlayerInfoReq , playerId = 1001}; // 서버에 붙자마자 OnConnected가 실행되면 플레이어 정보를 받는다
            //사실 보면 크기가 지금 아래를 보면 완전 달라진다 그래서 크기는 모든 크기를 계산하고 마지막에 넣는게 정석이다


            // 보낸다 (서버랑 반대)
            //for (int i = 0; i < 5; i++)
            {
                ArraySegment<byte> s = SendBufferHelper.Open(4096); // 원하는 사이즈를 예약해서 공간 확보하는건 동일하다(자주 나오니 이름 변경했다네)

                //이부분 너무 찜찜해 넣어준 크기만큼 new를 해서 바이트 배열을 동적으로 연결해줬다는거잖아 그냥 위에 ArraySegment에 바로 넣어주면 좋을텐데(이 버번에서는 불가능)
                //이를 해결하기 위한 많은 방법이 있다 유니티에서 가능한지 하나씩 체크해봐야 한다 다른거 확인하려면 BitConverter. 눌러서 체크
                /*bool success = true;
                ushort count = 0;
                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), packet.size); // 유니티에서도 되는지는 확인해봐야 한다
                count += 2;
                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), packet.packetId); // 유니티에서도 되는지는 확인해봐야 한다
                count += 2;
                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), packet.playerId); // 유니티에서도 되는지는 확인해봐야 한다
                count += 8;*/

                //크기는 이제 마지막에 넣어주는게 맞다

                bool success = true;
                ushort count = 0;
                 // 유니티에서도 되는지는 확인해봐야 한다
                count += 2; //걱정마 이것도 자동화 해줄거야
                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), packet.packetId); // 유니티에서도 되는지는 확인해봐야 한다
                count += 2;
                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), packet.playerId); // 유니티에서도 되는지는 확인해봐야 한다
                count += 8;
                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count);




                //이렇게 되면 이 아래는 전부 사용할 필요가 없어진다
                /*byte[] size = BitConverter.GetBytes(packet.size); // 2byte
                byte[] packetId = BitConverter.GetBytes(packet.packetId);//이 부분 나중에 수정 2byte
                byte[] PlayertId = BitConverter.GetBytes(packet.playerId); // 8byte

                ushort count = 0; // 버퍼를 얼마나 넣었는지 추적하는 바이트

                //계속 다음 위치로 복사하기 위해서 사용한 바이트를 파악하고 있어야 한다.
                *//*                Array.Copy(size, 0, openSegment.Array, openSegment.Offset + 0, 2);
                                Array.Copy(packetId, 0, openSegment.Array, openSegment.Offset + 2, 2);
                                ArraySegment<byte> sendBuff = SendBufferHelper.Close(12); 이렇게 위와같이 사이즈를 가지게 될것*//*

                Array.Copy(size, 0, s.Array, s.Offset + count, 2);
                count += 2;
                Array.Copy(packetId, 0, s.Array, s.Offset + count, 2);
                count += 2;
                Array.Copy(PlayertId, 0, s.Array, s.Offset + count, 2);
                count += 8;*/
                ArraySegment<byte> sendBuff = SendBufferHelper.Close(12);


                //이때 sendBuff가 보내줘야하는 버퍼가 만들어진다.
                if (success) 
                    Send(sendBuff);
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer) // 현재 하는 작업은 엔진과 컨텐츠를 분리하는 작업
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count); // 어디서부터 시작하냐 Offset
            Console.WriteLine($"[From Server]{recvData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred byte {numOfBytes}");
        }
    }
}
