using DummyClient;
using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClinet // 현재까지는 한번만 접속하고 있고 유니티 사용할때도 똑같은게 여러개가 될 일은 없다 지금은 하나를 여러개 써야 하니 지금은 세션 매니저로 관리
{
    
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
            connector.Connect(endPoint,
                () =>
                {
                    return SessionManager.Instance.Generate();
                }, 500); //  N개 생성
            //클라이언트 연결을 늘리면 서버 메모리가 증가하는 문제 발생(패킷 분실이 있을 수 있음)
            //개선을 위해 하나씩 패킷을 보내고 있는데 500명에게 보낸다면 25000번을 계속 보내야 한다는 의미
            //패킷을 모아서 보내면 N ^ 2 의 부화를 N까지 줄일 수 있다. 


            while (true)
            {
                try
                {
                    SessionManager.Instance.SendForEach();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                Thread.Sleep(250); // 일반적으로 MMO에서 패킷을 1초에 4번정도 보내기에
            }
        }
    }
}