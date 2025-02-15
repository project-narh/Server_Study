using System;
using System.Collections.Generic;
using System.Text;

namespace PacketGenerator
{
    class PacketFormat
    {
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

interface IPacket
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