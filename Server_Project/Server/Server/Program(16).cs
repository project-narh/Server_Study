//using System;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using static System.Collections.Specialized.BitVector32;
//using ServerCore; // 추가 서버 코어를 라이브러리화하였기 때문에

////이로서 서버 코어는 엔진    서버는 콘텐츠를 관리할 수 있게 되었다
//// 세션 인터페이스만 사용하고 이벤트만 지정해서 사용하고 있다 현재

//namespace Server
//{
//    class GameSession : Session
//    {
//        public override void OnConnected(EndPoint endPoint)
//        {
//            byte[] sendBuff = Encoding.UTF8.GetBytes("Welcom to MMORPG Server !");
//            Send(sendBuff);
//            Thread.Sleep(1000);
//            Disconnect();
//            Console.WriteLine($"OnConnected {endPoint}");
//        }

//        public override void OnDisconnected(EndPoint endPoint)
//        {
//            Console.WriteLine($"OnDisconnected {endPoint}");
//        }

//        public override int OnRecv(ArraySegment<byte> buffer) // 현재 하는 작업은 엔진과 컨텐츠를 분리하는 작업
//        {
//            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count); // 어디서부터 시작하냐 Offset
//            Console.WriteLine($"[From client]{recvData}");
//            return buffer.Count;
//        }

//        public override void OnSend(int numOfBytes)
//        {
//            Console.WriteLine($"Transferred byte {numOfBytes}");
//        }
//    }


//    class Program
//    {
//        static Listener _listener = new Listener();

//        static void Main(string[] args)
//        {
//            string host = Dns.GetHostName();
//            IPHostEntry ipHost = Dns.GetHostEntry(host);
//            IPAddress ipAddr = ipHost.AddressList[0];
//            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

//            _listener.Init(endPoint, () => { return new GameSession(); }); // 람다로 간단히 처리

//            Console.WriteLine("Listening...");

//            //영업을 한 번만 하고 마는게 아니니까 무한 루프
//            while (true)
//            {
//                ;
//                //프로그램이 종료되지 않게
//            }

//        }
//    }
//}