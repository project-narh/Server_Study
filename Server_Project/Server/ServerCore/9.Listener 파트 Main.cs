using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Program
    {
        static Listener _listener = new Listener();

        static void OnAcceptHandler(Socket clientSocket)
        {
            try
            {
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost =  Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);
            
            _listener.Init(endPoint, OnAcceptHandler);
            Console.WriteLine("Listening...");

            //영업을 한 번만 하고 마는게 아니니까 무한 루프
            while (true)
            {
                ;
                //프로그램이 종료되지 않게
            }

        }
    }

}