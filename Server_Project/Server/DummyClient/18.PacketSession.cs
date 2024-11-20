//using ServerCore;
//using System;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;

//namespace DummyClinet
//{
//    class Packet
//    {
//        public ushort size;
//        public ushort packetId;
//        //int로 하는것보다 4바이트를 아깔 수 있다.
//    }
//    //메모리상에 인스턴스로 존제하는 데이터를 납작하게 눌러서 버퍼에 넣는 작업 = 직렬화 
//    //BYTE -> 인스턴스 역직렬화
//    //일반적으로 세이브 파일을 만든다고 하면 파일로 만들어서 저장할거다 이도 바이트로 만들어서 보낼텐데 이것도 직렬화

//    class GameSession : Session
//    {
//        public override void OnConnected(EndPoint endPoint)
//        {
//            Console.WriteLine($"OnConnected {endPoint}");
//            // 보낸다 (서버랑 반대)
//            for (int i = 0; i < 5; i++)
//            {
//                Packet packet = new Packet() { size = 4, packetId = 7 };
//                ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
//                byte[] buffer = BitConverter.GetBytes(packet.size);
//                byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
//                Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length); // buffer를 0에서부터 buffer길이만큼 sendbuff로 0에서부터 카피 
//                Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);// buffer2를 0에서부터 buffer2길이만큼 sendbuff로 buffer 길이 이후로 카피
//                                                                                                              //이러면 만들어논 세그먼트에 데이터를 넣는다. 위와같이 하면 총 8byte를 사용했고 이를 닫아야한다.
//                ArraySegment<byte> sendBuff = SendBufferHelper.Close(packet.size); // 버퍼를 닫으면서 사용한 크기 전달         
//                //이때 sendBuff가 보내줘야하는 버퍼가 만들어진다.
//                Send(sendBuff);
//            }
//        }

//        public override void OnDisconnected(EndPoint endPoint)
//        {
//            Console.WriteLine($"OnDisconnected {endPoint}");
//        }

//        public override int OnRecv(ArraySegment<byte> buffer) // 현재 하는 작업은 엔진과 컨텐츠를 분리하는 작업
//        {
//            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count); // 어디서부터 시작하냐 Offset
//            Console.WriteLine($"[From Server]{recvData}");
//            return buffer.Count;
//        }

//        public override void OnSend(int numOfBytes)
//        {
//            Console.WriteLine($"Transferred byte {numOfBytes}");
//        }
//    }


//    class Program
//    {
//        static void Main(string[] args)
//        {
//            string host = Dns.GetHostName(); // 내 로컬 호스트의 도메인
//            IPHostEntry ipHost = Dns.GetHostEntry(host);
//            IPAddress ipAddr = ipHost.AddressList[0];
//            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);// 각 정문과 후문의 번호를 나타냄(주소와 포트)

//            //리스너를 사용했던거 처럼 커넥트를 사용한다
//            Connector connector = new Connector();
//            connector.Connect(endPoint, () => { return new GameSession(); });

//            while(true)
//            {
//                /*//휴대폰 설정
//                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
//*/
//                try
//                { 
//                    //이제 이 아래는 필요가 없다
//                    /*//문지기에게 입장 문의
//                    socket.Connect(endPoint);//문지기에게 연결 시도 // 이렇게 접속 시도하는걸 커텍터로 변경할것
//                    Console.WriteLine($"conneted To {socket.RemoteEndPoint.ToString()}"); //연결한 반대쪽 대상

//                    //이제 필요없다 세션에 리시브를 논블로킹으로 만들어놨다.
//                    *//*// 받는다
//                    byte[] recvBuff = new byte[1024];
//                    int recvBytes = socket.Receive(recvBuff); //현재 블로킹중
//                    String recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
//                    Console.WriteLine($"[From Server]{recvData}");*//*


//                    //나간다
//                    socket.Shutdown(SocketShutdown.Both);
//                    socket.Close();*/
//                }
//                catch (Exception e)
//                {
//                    Console.WriteLine(e.ToString());
//                }
//                Thread.Sleep(1000);
//            }
//        }
//    }
//}