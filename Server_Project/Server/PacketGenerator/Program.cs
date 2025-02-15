using System;
using System.Diagnostics;
using System.Xml;

namespace PacketGenerator
{
    //패킷 정의는 JSON XML 원하는거 어떤거든 상관 X
    class Program
    {
        static string genPackets; // 만들어지는 패킷들을 저장한다
        static ushort packetId;
        static string packetEnums; // 패킷을 몇개 처리했는지 기억해야 해서 위 변수
        static void Main(string[] args)
        {
            string pdlPath = "../PDL.xml";

            XmlReaderSettings settings = new XmlReaderSettings() // XML 동작방식 설정
            {
                IgnoreComments = true, // 주석 무시
                IgnoreWhitespace = true, // 공백 무시

            };
            if(args.Length >= 1)
                pdlPath = args[0];

            using (XmlReader r = XmlReader.Create(pdlPath, settings)) //경로 지정해줘야함 실행파일이 있는 경로를 기준으로 실행됨 (이제 인자로 받을 수 있게)
            {
                r.MoveToContent();

                while (r.Read())
                {
                    if (r.Depth == 1 && r.NodeType == XmlNodeType.Element) // 시작하는 부분 종료는 EndElement
                        ParsePacket(r);
                    //Console.WriteLine(r.Name +  "  " + r["name"]);
                }
                string fileText = string.Format(PacketFormat.fileFormat,packetEnums,genPackets);
                File.WriteAllText("GenPackets.cs", fileText);
            }
        }
        static void ParsePacket(XmlReader r) // 하나의 패킷 단위니 여기서 Enum도 진행
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

/*            Console.WriteLine($"packetName: {packetName}");
            Console.WriteLine($"t.Item1: {t.Item1}");
            Console.WriteLine($"t.Item2: {t.Item2}");
            Console.WriteLine($"t.Item3: {t.Item3}");*/

            try
            {
                genPackets += string.Format(PacketFormat.packetFormat, packetName, t.Item1, t.Item2, t.Item3);
                packetEnums += string.Format(PacketFormat.packetEnumFormat, packetName, ++packetId) + Environment.NewLine + "\t";
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
                    //byte와 sbyte 추가
                    case "byte":// 기존에는 ToIn64로 얻고 있는데 바이트 배열인데 이렇게 할 필요가 있을까? 그래서 수정 
                    case "sbyte":
                        memeberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readByteFormat, memberName, memberType); // 변환할 필요 없이 넣으면 된다 지금 가진게 바이트니까
                        writeCode += string.Format(PacketFormat.writeByteFormat, memberName, memberType);
                        break;
                    case "bool":
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
                        Tuple<string, string, string> t = ParseList(r);
                        memeberCode += t.Item1;
                        readCode += t.Item2;
                        writeCode += t.Item3;
                        break;
                    default:
                        break;

                }
            }
            memeberCode = memeberCode.Replace("\n", "\n\t");
            readCode = readCode.Replace("\r", "\n\t\t"); // 코드 정리
            writeCode = writeCode.Replace("\n", "\n\t\t");
            return new Tuple<string, string, string>(memeberCode, readCode, writeCode);

        }

        public static Tuple<string, string, string> ParseList(XmlReader r)
        {
            string listName = r["name"];
            Console.WriteLine(listName);
            if(string.IsNullOrEmpty(listName))
            {
                Console.WriteLine("List without name");
                return null;
            }
            Tuple<string, string, string> t = ParseMembers(r);
            string memberCode = string.Format(PacketFormat.memberListFormat,
                 FirstCharToUpper(listName),
                 FirstCharToLower(listName),
                 t.Item1,
                 t.Item2,
                 t.Item3);
            Console.WriteLine(FirstCharToUpper(listName));
            Console.WriteLine(FirstCharToLower(listName));
            Console.WriteLine(t.Item1);
            Console.WriteLine(t.Item2);
            Console.WriteLine(t.Item3);

            string readCode = string.Format(PacketFormat.readListFormat,
                FirstCharToUpper(listName),
                FirstCharToLower(listName));
            string writeCode = string.Format(PacketFormat.writeListFormat,
               FirstCharToUpper(listName),
               FirstCharToLower(listName));
            return new Tuple<string, string, string>(memberCode, readCode, writeCode);

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
        public static string FirstCharToUpper(String input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return input[0].ToString().ToUpper() + input.Substring(1);
        }

        public static string FirstCharToLower(String input) // 모든 애들이 다 소문자(첫 캐릭터만 소문자로)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return input[0].ToString().ToLower() + input.Substring(1);
        }
    }
}