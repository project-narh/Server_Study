using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //1. 문지기를 만들어야 한다 리슨 소켓(listenSocket)
            //DNS (Domain name System)
            //172.1.2.3 이렇게 넣으면 문제가 되는게 나중에 상황에 따라 서버가 이전되면 IP도 바뀔 수 있다
            //주소로 하드코딩해서 하면 자동 처리가 안되지만 도메인을 등록하고 ip를 따로 관리하면 이름으로 주소를 찾아서 관리
            string host = Dns.GetHostName(); // 내 로컬 호스트의 도메인
            IPHostEntry ipHost =  Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);// 각 정문과 후문의 번호를 나타냄(주소와 포트)
                                                               //ipHost.AddressList 배열로 원하는 주소 반환 왜 리스트냐 트래픽이 많은곳은 하나의 아이피가 아니라 여러개를 관리하여 부하 관리

            //문지기
            Socket listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp); // TCP로 통신 (문지기가 들고 있는 휴대폰)

            try
            {
                // 문지기 교육 (bind)
                listenSocket.Bind(endPoint);

                //영업 시작
                // backlog : 최대 대기수 숫자를 초과하면 Fail 뜬다
                listenSocket.Listen(10);

                //영업을 한 번만 하고 마는게 아니니까 무한 루프
                while (true)
                {
                    Console.WriteLine("Listening...");

                    // 손입 입장
                    Socket clientSocket = listenSocket.Accept(); // 손님과 대화하려면 이 소켓으로 진행
                    //(만약 안들어오면 어떻게 되나? 그건 블로킹 논블로킹이라는 개념이 존재 안들어오면 이 아래는 진행이 안되는 상태)

                    //받는다
                    byte[] recvBuff = new byte[1024];
                    int recvBytes = clientSocket.Receive(recvBuff); // 데이터는 recvBuff   몇 바이트인지는  recvBytes에 저장
                    string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);//문자열을 쓸꺼니 규약을 정하는거 문자열을 받는다고 가정하는거라 이렇게하는거
                    Console.WriteLine($"[From clinet] {recvData}");

                    // 보낸다
                    byte[] sendBuff = Encoding.UTF8.GetBytes("Welcom to MMORPG Server !");
                    clientSocket.Send(sendBuff);

                    //쫓아낸다
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }

}