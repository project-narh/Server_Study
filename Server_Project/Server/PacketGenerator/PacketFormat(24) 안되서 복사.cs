//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PacketGenerator
//{
//    internal class PacketFormat
//    {
//        // {0} 패킷이름
//        // {1} 맴버 변수
//        // {2} 맴버 번수 Read
//        // {3} 맴버 변수 Write
//        //일단 리스트는 복잡하니 빼자
//        //공통적인 부분은 남기고 바뀌는 부분들 제거하고 번호 붙이는 작업
//        public static string packetFormat =
//@"
//class {0}
//{{
//    {1}


//    public  void Read(ArraySegment<byte> segment)
//    {{
//        ushort count = 0;
//        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
//        count += sizeof(ushort);
//        count += sizeof(ushort);
//        {2}
//    }

//    public ArraySegment<byte> Write()
//    {{
//        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
//        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
//        bool success = true;
//        ushort count = 0;

//        count += sizeof(ushort);
//        success &= BitConverter.TryWriteBytes(s.Slice(count,s.Length - count), (ushort)PacketID.{0});
//        count += sizeof(ushort);

//        {3}
//        success &= BitConverter.TryWriteBytes(s, count);

//        if (!success) return null;
//        return SendBufferHelper.Close(count);
//    }}
//}";
//        // {0} 변수 형식
//        // {1} 변수 이름
//        public static string memberFormat =
//@"public {0} {1};";

//        // {0} 변수 이름
//        // {1} To~ 변수 형식
//        // {2} 변수 형식
//        public static string readFormat =
//@"this.{0} = BitConverter.{1}(s.Slice(count, s.Length - count));
//count += sizeof({2});";


//        //string은 양식이 달라서
//        // {0} 변수 이름
//        public static string readStringFormat =
//@"ushort {0}Len = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
//count += sizeof(ushort);
//this.{0} = Encoding.Unicode.GetString(s.Slice(count, {0}Len));
//count += {0}Len;";

//        // {0} 변수 이름
//        // {1} 변수 형식
//        public static string writeFormat =
//@"success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.{0});
// count += sizeof({1});";


//        public static string writeStringFormat =
//@"ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.{0}, 0, this.{0}.Length,segment.Array,segment.Offset + count + sizeof(ushort));
//success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), {0}Len);
//count += sizeof(ushort);
//count += {0}Len;";
//    }
//}
