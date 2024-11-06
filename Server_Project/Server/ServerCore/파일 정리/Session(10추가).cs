/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    internal class Session
    {
        Socket _socket;
        int _disconnected = 0;
        public void Start(Socket socket)
        {
            _socket = socket;
            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            recvArgs.SetBuffer(new byte[1024],0,1024);
            //이렇게 하는 이유는 버퍼를 엄청 크게 나눠서 세션끼리 나눠쓰는 경우가 있기 때문
            //이렇게 하면 세션을 만들때 마다 버퍼를 만듬
            //추가 정보를 주고 싶으면
            //recvArgs.UserToken = 어떤 정보든 상관 없음; 식별자나 연동하고 싶은 데이터가 있을대
            //SetBuffer로 해줘야한다 기존에 리슨때는 args.AcceptSocket = null;
            RegisterRecv(recvArgs);
        }

        public void Send(byte[] sendBuff)
        {
            _socket.Send(sendBuff);
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
        //동시다발적 혹은 동시에 2개가 실행이 되면?

        #region 네트워크
        void RegisterRecv(SocketAsyncEventArgs args)
        {
            bool pending = _socket.ReceiveAsync(args);
            if (!pending) OnRecvCompleted(null, args);
        }
        
        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            //이부분은 좀 다름
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success) // 몇 바이트를 받았느냐
            {
                //TODO
                try
                {
                    string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred); // 어디서부터 시작하냐 Offset
                    Console.WriteLine($"[From client]{recvData}");
                    RegisterRecv(args);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            else
            {
                Disconnect();
            }
        }
    }
    #endregion
}
*/