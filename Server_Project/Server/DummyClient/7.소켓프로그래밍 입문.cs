/*using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName(); // 내 로컬 호스트의 도메인
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);// 각 정문과 후문의 번호를 나타냄(주소와 포트)
            
            while(true)
            {
                //휴대폰 설정
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                { //문지기에게 입장 문의
                    socket.Connect(endPoint);//문지기에게 연결 시도
                    Console.WriteLine($"conneted To {socket.RemoteEndPoint.ToString()}"); //연결한 반대쪽 대상

                    // 보낸다 (서버랑 반대)
                    byte[] sendBuff = Encoding.UTF8.GetBytes("Hello World!");
                    int sendBytes = socket.Send(sendBuff);

                    // 받는다
                    byte[] recvBuff = new byte[1024];
                    int recvBytes = socket.Receive(recvBuff);
                    String recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                    Console.WriteLine($"[From Server]{recvData}");

                    //나간다
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                Thread.Sleep(1000);
            }
        }
    }
}*/