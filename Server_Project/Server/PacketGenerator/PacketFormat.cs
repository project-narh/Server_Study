using System;
using System.Collections.Generic;
using System.Text;

namespace PacketGenerator
{
    class PacketFormat
    {
        // {0} 패킷 등록
        public static string managerFormat =
@"using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{{
    #region Singleton (이전 방식은 매번 실행될때 래지스터 호출해줘야 해서 그런 작업 안하게 수정)
    static PacketManager _instance = new PacketManager();
    public static PacketManager Instance {{ get {{ return _instance; }} }}
    #endregion
    
    PacketManager() 
    {{
        Register();
    }}
    Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _makeFunc = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();//프로토콜ID, 행동 반환값이 있으니 이제 Action이 아닌 Func
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

    public void Register()
    {{
        //패킷을 받는도중 Register를 하면 문제 발생 (먼저 등록하고 패킷이 들어오면 문제가 안된다)
{0}
    }}

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallback = null)// 유효한 범위를 보내주는거
    {{
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
        //{{
        //    case PacketID.PlayerInfoReq:
        //        {{
        //            //long playerId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        //            //count += 8;
        //            PlayerInfoReq p = new PlayerInfoReq();
        //            p.Read(buffer);

        //        }}
        //        break;
        //아래 코드로 변경
        //}}*/

        Func<PacketSession, ArraySegment<byte>, IPacket> func = null;
        if(_makeFunc.TryGetValue(id, out func))
        {{
            IPacket packet =  func.Invoke(session, buffer);
            //이게 실행되고 위에 딕셔너리로 등록한 핸들러에 접근되고 그 핸들러가 MakePacket 메소드를 실행한다
            if (onRecvCallback != null) onRecvCallback.Invoke(session, packet);
            else HandlePacket(session, packet);
        }}

    }}

    T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {{
        /*long playerId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
                    count += 8;*/
        T pkt = new T();
        pkt.Read(buffer);
        return pkt;
    }}

    // 핸들러로 보내는 부분 분리
    public void HandlePacket(PacketSession session, IPacket packet)
    {{
        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(packet.Protocol, out action))
            action.Invoke(session, packet);
    }}
}}
";
        // {0} 패킷 이름
        public static string mangerRegisterFormat =
@"
        _makeFunc.Add((ushort)PacketID.{0}, MakePacket<{0}>);
        _handler.Add((ushort)PacketID.{0}, PacketHandler.{0}Handler);";

        //{0} 패킷 이름/번호 목록
        //{1} 패킷 목록
        public static string fileFormat =
@"
using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

 public enum PacketID // 지금은 이렇게 하드코딩 하지만 나중에는 자동화
{{
    {0}
}}

public interface IPacket
{{ 
	ushort Protocol {{ get; }}
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}}

{1}
";

        public static string packetEnumFormat =
@"{0} = {1},";

        // {0} 패킷 이름
        // {1} 멤버 변수
        // {2} 멤버 변수 Read
        // {3} 멤버 변수 Write

        // 여러줄에 걸쳐서 문자열을 정의해 주고 싶을 땐 @를 붙이면 된다. 
        public static string packetFormat =
@"class {0} : IPacket
{{
    {1}     

	public ushort Protocol {{ get {{ return (ushort) PacketID.{0}; }} }}

    public void Read(ArraySegment<byte> segment)
    {{
        ushort count = 0;

        ReadOnlySpan<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        {2}
    }}

    public ArraySegment<byte> Write()
    {{
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0; 
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.{0});
        count += sizeof(ushort);
        {3}
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }}
}}
";
        // {0} 변수 형식
        // {1} 변수 이름
        public static string memberFormat =
@"public {0} {1};";

        // {0} 리스트이름[대문자]
        // {1} 리스트이름[소문자]
        // {2} 멤버 변수
        // {3} 멤버 변수 Read
        // {4} 멤버 변수 Write

        // {0} 리스트 이름 [대문자]
        // {1} 리스트 이름 [소문자]
        // {2} 멤버 변수
        // {3} 멤버 변수 Read
        // {4} 멤버 변수 Write
        public static string memberListFormat =
@"
public class {0}
{{
{2}

public void Read(ReadOnlySpan<byte> s, ref ushort count)
{{
    {3}
}}

public bool Write(Span<byte> s, ref ushort count)
{{
    bool success = true;
    {4}
    return success;
}}
}}
public List<{0}> {1}s = new List<{0}>();";

        // {0} 변수 이름
        // {1} To~ 변수 형식
        // {2} 변수 형식
        public static string readFormat =
@"this.{0} = BitConverter.{1}(s.Slice(count, s.Length - count));
count += sizeof({2});";

        //byte만 예외적으로 처리
        // {0} 변수 이름
        // {1} 변수 형식
        // 변수 형식 받는 건 Sbyte 대비 

        public static string readByteFormat =
@"this.{0} = ({1})segment.Array[segment.Offset + count];
count += sizeof({1});";
        //@"segment.Array[segment.Offset + count] = this.testByte;
        //count += sizeof(byte);"; 이렇게 해줘도 되지만 sbyte를 따로 하고 싶을 수 있기 때문에 따로 처리해준다

        //byte만 예외적으로 처리
        // {0} 변수 이름
        // {1} 변수 형식

        public static string writeByteFormat =
@"segment.Array[segment.Offset + count] = (byte)this.{0};
count += sizeof({1});";
        //@"segment.Array[segment.Offset + count] = this.testByte;
        //count += sizeof(byte);"; 이렇게 해줘도 되지만 sbyte를 따로 하고 싶을 수 있기 때문에 따로 처리해준다

        // {0} 변수 이름
        public static string readStringFormat =
@"ushort {0}Len = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
count += sizeof(ushort);
this.{0} = Encoding.Unicode.GetString(s.Slice(count, {0}Len));
count += {0}Len;";

        //        // {0} 리스트 이름 대문자
        //        // {1} 리스트 이름 소문자
        //        //
        //        public static string readListFormat =
        //@"this.{1}s.Clear();//다른 정보 가지고 있었을 수 있으니
        //ushort {1}Len = BitConverter.ToUInt16(s.Slice(count, s.Length - count));// 스킬 갯수 추출
        //count += sizeof(ushort);

        //for(int i = 0; i < {1}Len; i++)
        //{
        //    {0} {1} = new {0}();
        //    {1}.Read(s, ref count);
        //    {1}s.Add({1});
        //}";

        // {0} 리스트 이름 [대문자]
        // {1} 리스트 이름 [소문자]
        public static string readListFormat =
@"this.{1}s.Clear();
ushort {1}Len = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
count += sizeof(ushort);
for (int i = 0; i < {1}Len; i++)
{{
    {0} {1} = new {0}();
    {1}.Read(s, ref count);
    {1}s.Add({1});
}}";


        // {0} 변수 이름
        // {1} 변수 형식
        public static string writeFormat =
@"success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.{0});
count += sizeof({1});";

        // {0} 변수 이름
        public static string writeStringFormat =
@"ushort {0}Len = (ushort)Encoding.Unicode.GetBytes(this.{0}, 0, this.{0}.Length, segment.Array, segment.Offset + count + sizeof(ushort));
success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), {0}Len);
count += sizeof(ushort);
count += {0}Len;";

/*        // {0} 리스트 이름 대문자
        // {1} 리스트 이름 소문자

        public static string writeListFormat =
@"success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort){1}s.Count); // 2byte로 넣어줘야겠지?
count += sizeof(ushort);
foreach({0} {1} in {1}s)
{{
    //TODO
    success &= {1}.Write(s, ref count);
}}
";*/
        // {0} 리스트 이름 [대문자]
        // {1} 리스트 이름 [소문자]        
        public static string writeListFormat =
@"success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.{1}s.Count);
count += sizeof(ushort);
foreach ({0} {1} in this.{1}s)
success &= {1}.Write(s, ref count);";
    }
}