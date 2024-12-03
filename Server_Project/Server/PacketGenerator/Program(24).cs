using System;
using System.Xml;

namespace PacketGenerator
{
    //패킷 정의는 JSON XML 원하는거 어떤거든 상관 X
    class Program
    {
        static string genPackets; // 만들어지는 패킷들을 저장한다

        static void Main(string[] args)
        {
            XmlReaderSettings settings = new XmlReaderSettings() // XML 동작방식 설정
            {
                IgnoreComments = true, // 주석 무시
                IgnoreWhitespace = true, // 공백 무시

            };
            using (XmlReader r = XmlReader.Create("PDL.xml", settings)) //경로 지정해줘야함 실행파일이 있는 경로를 기준으로 실행됨
            {
                r.MoveToContent();

                while (r.Read())
                {
                    if (r.Depth == 1 && r.NodeType == XmlNodeType.Element) // 시작하는 부분 종료는 EndElement
                        ParsePacket(r);
                    //Console.WriteLine(r.Name +  "  " + r["name"]);
                }
                File.WriteAllText("GenPackets.cs", genPackets);
            }
        }
        static void ParsePacket(XmlReader r)
        {
            if (r.NodeType == XmlNodeType.EndElement) return;
            if (r.Name.ToLower() != "packet")
            {
                Console.WriteLine("Invalid packet node");
                return;
            }

            string packetName = r["name"];
            if (string.IsNullOrEmpty(packetName))
            {
                Console.WriteLine("Packet witout name");
                return;
            }

            // {0} 패킷이름
            // {1} 맴버 변수
            // {2} 맴버 번수 Read
            // {3} 맴버 변수 Write
            Tuple<string, string, string> t = ParseMembers(r);


            if (t == null || string.IsNullOrEmpty(t.Item1) || string.IsNullOrEmpty(t.Item2) || string.IsNullOrEmpty(t.Item3))
            {
                Console.WriteLine($"Error parsing members for packet: {packetName}");
                return;
            }

            Console.WriteLine($"packetName: {packetName}");
            Console.WriteLine($"t.Item1: {t.Item1}");
            Console.WriteLine($"t.Item2: {t.Item2}");
            Console.WriteLine($"t.Item3: {t.Item3}");

            try
            {
                genPackets += string.Format(PacketFormat.packetFormat, packetName, t.Item1, t.Item2, t.Item3);
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"FormatException in packet: {packetName}");
                Console.WriteLine($"Error Details: {ex.Message}================");

                Console.WriteLine($"FormatException in packet: {packetName}");
                Console.WriteLine($"packetName: {packetName}");
                Console.WriteLine($"t.Item1: {t.Item1}");
                Console.WriteLine($"t.Item2: {t.Item2}");
                Console.WriteLine($"t.Item3: {t.Item3}");
                throw;
            }
        }

        public static Tuple<string, string, string> ParseMembers(XmlReader r)
        {
            string packetName = r["name"];

            string memeberCode = "";
            string readCode = "";
            string writeCode = "";
            int depth = r.Depth + 1;
            while (r.Read())
            {
                if (r.Depth != depth)
                    break;
                string memberName = r["name"];
                if (string.IsNullOrEmpty(memberName))
                {
                    Console.WriteLine("Member without name");
                    return null;
                }

                if (string.IsNullOrEmpty(memeberCode) == false)
                    memeberCode += Environment.NewLine; // 한칸 엔터
                if (string.IsNullOrEmpty(readCode) == false)
                    memeberCode += Environment.NewLine; // 한칸 엔터'
                if (string.IsNullOrEmpty(writeCode) == false)
                    memeberCode += Environment.NewLine; // 한칸 엔터

                string memberType = r.Name.ToLower();

                Console.WriteLine($"{memberName}  {memberType}");
                switch (memberType)
                {
                    case "bool":
                    case "byte":
                    case "short":
                    case "ushort":
                    case "int":
                    case "long":
                    case "float":
                    case "double":
                        memeberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readFormat, memberName, ToMemberType(memberType), memberType);
                        writeCode += string.Format(PacketFormat.writeFormat, memberName, memberType);
                        break;
                    case "string":
                        memeberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readStringFormat, memberName);
                        writeCode += string.Format(PacketFormat.writeStringFormat, memberName);
                        break;
                    case "list":
                    default:
                        break;

                }
            }
            return new Tuple<string, string, string>(memeberCode, readCode, writeCode);

        }

        public static string ToMemberType(string memberType)
        {
            switch (memberType)
            {
                case "bool":
                    return "ToBoolean";
                case "short":
                    return "ToInt16";
                case "ushort":
                    return "ToUInt16";
                case "int":
                    return "ToInt32";
                case "long":
                    return "ToInt64";
                case "float":
                    return "ToSingle";
                case "double":
                    return "ToDouble";
                default:
                    return "";

            }
        }
    }
}