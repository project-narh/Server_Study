using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClinet
{
    //현제 커넥트를 사용할때 세션 펙토리를 사용해야 하고 인보크를 써서 사용해야 하기에
    //기존에 사용하던 세션 클래스를 가져온다
    // 이때 달라지는건 보내는 작업이 달라지기에 이를 수정한다

    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected {endPoint}");
            // 보낸다 (서버랑 반대)
            for (int i = 0; i < 5; i++)
            {
                byte[] sendBuff = Encoding.UTF8.GetBytes($"Hello World!{i}");
                Send(sendBuff);
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected {endPoint}");
        }

        public override void OnRecv(ArraySegment<byte> buffer) // 현재 하는 작업은 엔진과 컨텐츠를 분리하는 작업
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count); // 어디서부터 시작하냐 Offset
            Console.WriteLine($"[From Server]{recvData}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred byte {numOfBytes}");
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName(); // 내 로컬 호스트의 도메인
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);// 각 정문과 후문의 번호를 나타냄(주소와 포트)

            //리스너를 사용했던거 처럼 커넥트를 사용한다
            Connector connector = new Connector();
            connector.Connect(endPoint, () => { return new GameSession(); });

            while(true)
            {
                /*//휴대폰 설정
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
*/
                try
                { 
                    //이제 이 아래는 필요가 없다
                    /*//문지기에게 입장 문의
                    socket.Connect(endPoint);//문지기에게 연결 시도 // 이렇게 접속 시도하는걸 커텍터로 변경할것
                    Console.WriteLine($"conneted To {socket.RemoteEndPoint.ToString()}"); //연결한 반대쪽 대상

                    //이제 필요없다 세션에 리시브를 논블로킹으로 만들어놨다.
                    *//*// 받는다
                    byte[] recvBuff = new byte[1024];
                    int recvBytes = socket.Receive(recvBuff); //현재 블로킹중
                    String recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                    Console.WriteLine($"[From Server]{recvData}");*//*


                    //나간다
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();*/
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                Thread.Sleep(1000);
            }
        }
    }
}