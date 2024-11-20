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
    public abstract class Packet
    {
        //사이즈와 아이디는 제거하고 만들어도 무관하다
        public ushort size;
        public ushort packetId;

        //패킷 만드는거 자도오하 할거임
        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> s);


        //int로 하는것보다 4바이트를 아깔 수 있다.
    }
    class PlayerInfoReq : Packet
    {
        public long playerId;
        public string name;

        //그냥 리스트도 기존 데이터 처럼 반복으로 처리해주면 되겠지만 문제는 구조체 형식으로 리스트를 만드는 경우이다
        //일단 확실한건 리스트가 몇개인지 보내고 데이터를 넣으면 될것

        public struct SkillInfo
        {
            public int id;
            public short level;
            public float duration;

            public bool Write(Span<byte> s, ref ushort count) // 전체크기와 몇번째를 작업하고 있는지
            {
                bool success = true;
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.id);
                count += sizeof(int);
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.level);
                count += sizeof(short);
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.duration);
                count += sizeof(float);
                return success;
            }

            public void Read(ReadOnlySpan<byte> s, ref ushort count)
            {
                id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
                count += sizeof(int);
                id = BitConverter.ToInt16(s.Slice(count, s.Length - count));
                count += sizeof(short);
                duration = BitConverter.ToSingle(s.Slice(count, s.Length - count)); // ToSingle float이다
                count += sizeof(float);
            }
        }

        public List<SkillInfo> skills = new List<SkillInfo>();


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
            this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length- count));
            count += sizeof(long);

            //string
            ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);
            this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
            count += nameLen;

            //skill list
            skills.Clear();//다른 정보 가지고 있었을 수 있으니
            ushort skillLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));// 스킬 갯수 추출
            count += sizeof(ushort);

            for(int i = 0; i < skillLen; i++)
            {
                SkillInfo skillInfo = new SkillInfo();
                skillInfo.Read(s, ref count);
                skills.Add(skillInfo);
            }

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
            success &= BitConverter.TryWriteBytes(s.Slice(count,s.Length - count), this.packetId); // 이제는 자신이 패킷
            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
            count += sizeof(long); ;

            //string
            ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length,segment.Array,segment.Offset + count + sizeof(ushort)); // 위의 동작을 하나로 구현할 수 있다는 소리 nameLen 반환
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen); // 문자열 길이 저장
            count += sizeof(ushort); //위와 같이 하면 일단 길이도 보내줘야 하는데 먼저 다 해버려서 위도 그만큼 공간을 추가
            count += nameLen; //문자열 데이터 저장

            //skill list
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)skills.Count); // 2byte로 넣어줘야겠지?
            count += sizeof(ushort);
            foreach(SkillInfo skill in skills)
            {
                //TODO
                success &= skill.Write(s, ref count);
            }

            success &= BitConverter.TryWriteBytes(s, count);

            if (!success) return null;
            return SendBufferHelper.Close(count);
        }
    }


    public enum PacketID // 지금은 이렇게 하드코딩 하지만 나중에는 자도오하
    {
        PlayerInfoReq = 1, PlayerInfoOk = 2,
    }

    class ServerSession : Session
    {


        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected {endPoint}");
      
            PlayerInfoReq packet = new PlayerInfoReq() { playerId = 1001, name = "ABCDEFG" };//이제 아이디 크기 필요없음

            packet.skills.Add(new PlayerInfoReq.SkillInfo() { id = 101, level = 1, duration = 3.0f });
            packet.skills.Add(new PlayerInfoReq.SkillInfo() { id = 102, level = 2, duration = 4.0f });
            packet.skills.Add(new PlayerInfoReq.SkillInfo() { id = 103, level = 3, duration = 5.0f });
            packet.skills.Add(new PlayerInfoReq.SkillInfo() { id = 104, level = 4, duration = 6.0f });
            //사실 보면 크기가 지금 아래를 보면 완전 달라진다 그래서 크기는 모든 크기를 계산하고 마지막에 넣는게 정석이다

            ArraySegment<byte> s = packet.Write();
            if (s != null) Send(s);


           
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
