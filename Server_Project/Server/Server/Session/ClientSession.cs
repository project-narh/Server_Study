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
//    /*패킷을 구분하는 방법은 뭘까?
//ID로 1 이동 2 채팅 이런식으로 하는 방법이 있을 수 있다

//다만 문제는 경우에 따라 유동적으로 사이즈가 달라질 수 있다는 점이다
//그래서 첫 인자로 size 두번째로 ID를 넘겨주는 경우가 대다수이다. (int short 둘 중 ushort로 충분히 사용하긴 한다)*/
//    public abstract class Packet
//    {
//        //사이즈와 아이디는 제거하고 만들어도 무관하다
//        public ushort size;
//        public ushort packetId;

//        //패킷 만드는거 자도오하 할거임
//        public abstract ArraySegment<byte> Write();
//        public abstract void Read(ArraySegment<byte> s);


//        //int로 하는것보다 4바이트를 아깔 수 있다.
//    }
//    class PlayerInfoReq
//    {
//        public byte testByte;


//        public long playerId;


//        public string name;



//        public class Skill
//        {
//            public int id;


//            public short level;


//            public float duration;



//            public struct Attribute
//            {
//                public int att;

//                public void Read(ReadOnlySpan<byte> s, ref ushort count)
//                {
//                    this.att = BitConverter.ToInt32(s.Slice(count, s.Length - count));

//                    count += sizeof(int);
//                }

//                public bool Write(Span<byte> s, ref ushort count)
//                {
//                    bool success = true;
//                    success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.att);
//                    count += sizeof(int);
//                    return success;
//                }
//            }
//            public List<Attribute> attributes = new List<Attribute>();

//            public void Read(ReadOnlySpan<byte> s, ref ushort count)
//            {
//                this.id = BitConverter.ToInt32(s.Slice(count, s.Length - count));

//                count += sizeof(int); this.level = BitConverter.ToInt16(s.Slice(count, s.Length - count));

//                count += sizeof(short); this.duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));

//                count += sizeof(float); this.attributes.Clear();

//                ushort attributeLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));

//                count += sizeof(ushort);

//                for (int i = 0; i < attributeLen; i++)

//                {

//                    Attribute attribute = new Attribute();

//                    attribute.Read(s, ref count);

//                    attributes.Add(attribute);

//                }
//            }

//            public bool Write(Span<byte> s, ref ushort count)
//            {
//                bool success = true;
//                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.id);
//                count += sizeof(int); success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.level);
//                count += sizeof(short); success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.duration);
//                count += sizeof(float); success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.attributes.Count);
//                count += sizeof(ushort);
//                foreach (Attribute attribute in this.attributes)
//                    success &= attribute.Write(s, ref count);
//                return success;
//            }
//        }
//        public List<Skill> skills = new List<Skill>();

//        //public List<SkillInfo> skills = new List<SkillInfo>();

//        public void Read(ArraySegment<byte> segment)
//        {
//            ushort count = 0;

//            ReadOnlySpan<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

//            count += sizeof(ushort);
//            count += sizeof(ushort);
//            this.testByte = (byte)segment.Array[segment.Offset + count];

//            count += sizeof(byte); this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));

//            count += sizeof(long); ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));

//            count += sizeof(ushort);

//            this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));

//            count += nameLen; this.skills.Clear();

//            ushort skillLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));

//            count += sizeof(ushort);

//            for (int i = 0; i < skillLen; i++)

//            {

//                Skill skill = new Skill();

//                skill.Read(s, ref count);

//                skills.Add(skill);

//            }
//        }

//        public ArraySegment<byte> Write()
//        {
//            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
//            ushort count = 0;
//            bool success = true;

//            Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

//            count += sizeof(ushort);

//            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.PlayerInfoReq);
//            count += sizeof(ushort);
//            segment.Array[segment.Offset + count] = (byte)this.testByte;
//            count += sizeof(byte); success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
//            count += sizeof(long); ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
//            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
//            count += sizeof(ushort);
//            count += nameLen; success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.skills.Count);
//            count += sizeof(ushort);
//            foreach (Skill skill in this.skills)
//                success &= skill.Write(s, ref count);
//            success &= BitConverter.TryWriteBytes(s, count);
//            if (success == false)
//                return null;
//            return SendBufferHelper.Close(count);
//        }
//    }

//    /*    class PlayerInfoOk : Packet
//        {
//            public int hp;
//            public int attack;
//        }*/

//    public enum PacketID // 지금은 이렇게 하드코딩 하지만 나중에는 자도오하
//    {
//        PlayerInfoReq = 1, PlayerInfoOk = 2,
//    }
    
    class ClientSession : PacketSession //session > PacketSession
    {
        public int SessionId { get; set; }
        public GameRoom Room { get; set; }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected {endPoint}");
            /*//byte[] sendBuff = Encoding.UTF8.GetBytes("Welcom to MMORPG Server !");
            //Packet packet = new Packet() { size = 100, packetId = 10};
            //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            //byte[] buffer = BitConverter.GetBytes(packet.size);
            //byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
            //Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length); // buffer를 0에서부터 buffer길이만큼 sendbuff로 0에서부터 카피 
            //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);// buffer2를 0에서부터 buffer2길이만큼 sendbuff로 buffer 길이 이후로 카피
            //                                                                                              //이러면 만들어논 세그먼트에 데이터를 넣는다. 위와같이 하면 총 8byte를 사용했고 이를 닫아야한다.
            //ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length); // 버퍼를 닫으면서 사용한 크기 전달
            //                                                                                      //이때 sendBuff가 보내줘야하는 버퍼가 만들어진다.
            //Send(sendBuff);*/
            //강제로 채팅방에 입장시킨다(원래 바로 입장이 아닌 리소스 로딩이 끝나고 해야 하는게 맞다)

            //룸에 직접 접근했지만 이제 그러면 안된다
            Program.Room.Push(() => Program.Room.Enter(this));
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            SessionManager.Instance.Remove(this);
            if(Room != null)
            {
                GameRoom room = Room;
                room.Push(() => room.Leave(this));
                Room = null;
            }

            Console.WriteLine($"OnDisconnected {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer) // 유효한 범위를 보내주는거
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
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
            //Console.WriteLine($"Transferred byte {numOfBytes}");
        }
    }
}
