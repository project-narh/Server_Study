//using System;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;

//namespace ServerCore
//{
//    class Program
//    {
//        static Listener _listener = new Listener();

//        static void OnAcceptHandler(Socket clientSocket)
//        {
//            try
//            {
//                Session session = new Session();
//                session.Start(clientSocket);
//                byte[] sendBuff = Encoding.UTF8.GetBytes("Welcom to MMORPG Server !");
//                session.Send(sendBuff);

//                Thread.Sleep(1000);
//                session.Disconnect();

//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.ToString());
//            }
            
//        }
//        static void Main(string[] args)
//        {
//            string host = Dns.GetHostName();
//            IPHostEntry ipHost =  Dns.GetHostEntry(host);
//            IPAddress ipAddr = ipHost.AddressList[0];
//            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);
            
//            _listener.Init(endPoint, OnAcceptHandler);
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