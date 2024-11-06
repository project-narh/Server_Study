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

    //예를 들어 만든 클래스
    public class Knight
    {
        public int hp;
        public int attack;
        public string name;
        public List<int> skills = new List<int> { };
        //내용은 대충 넣은거라 변할 수 있고 가변적이다
    }


    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected {endPoint}");

            //위에 새로 만든 Knight를 보내려면 바이트로 변환해야하니 아래와 같다
            Knight knight = new Knight() { hp = 100, attack = 10 };

            /*//[100] [10]
            byte[] sendBuff = new byte[4096];//넉넉한 버퍼양 일부를 때어쓴다는 느낌
            byte[] buffer = BitConverter.GetBytes(knight.hp);
            byte[] buffer2 = BitConverter.GetBytes(knight.attack);
            Array.Copy(buffer, 0, sendBuff, 0, buffer.Length); // buffer를 0에서부터 buffer길이만큼 sendbuff로 0에서부터 카피 
            Array.Copy(buffer2, 0, sendBuff, buffer.Length, buffer2.Length);// buffer2를 0에서부터 buffer2길이만큼 sendbuff로 buffer 길이 이후로 카피 
            //현재 1~2명이면 모르겠는데 100명이면?
            // 한명이 이동해도 100명에게 이동 패킷을 보내야하는데 100명이 움직이면? 100*100 즉 1만의 데이터를 송신해야한다
            //그러면 이렇게 보내는거 무리무리*/


            //이렇게 버퍼를 채워야 한다. 이를 편하게 하기 위해
            //sendbuffer와 helper를 만들었고
            //아래는 예시다 이렇게 수동으로 만들어서 할 일은 없다 어떤 방식인지 보기 위한거지 다 패킷처리 할거야
            ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            byte[] buffer = BitConverter.GetBytes(knight.hp);
            byte[] buffer2 = BitConverter.GetBytes(knight.attack);
            Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length); // buffer를 0에서부터 buffer길이만큼 sendbuff로 0에서부터 카피 
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);// buffer2를 0에서부터 buffer2길이만큼 sendbuff로 buffer 길이 이후로 카피
            //이러면 만들어논 세그먼트에 데이터를 넣는다. 위와같이 하면 총 8byte를 사용했고 이를 닫아야한다.
            ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length); // 버퍼를 닫으면서 사용한 크기 전달
            //이때 sendBuff가 보내줘야하는 버퍼가 만들어진다.

            Send(sendBuff); // 이전에는 byte[]로 받았으니 이를 ArraySegment로 변경
            Thread.Sleep(1000);
            Disconnect();



            /*            // 보낸다 (서버랑 반대)
                        for (int i = 0; i < 5; i++)
                        {
                            byte[] sendBuff = Encoding.UTF8.GetBytes($"Hello World!{i}");
                            Send(sendBuff);
                        }*/
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer) // 현재 하는 작업은 엔진과 컨텐츠를 분리하는 작업
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count); // 어디서부터 시작하냐 Offset
            Console.WriteLine($"[From Server]{recvData}");
            return buffer.Count;
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