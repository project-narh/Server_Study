//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading.Tasks;

//namespace ServerCore
//{
//    internal class Session
//    {
//        Socket _socket;
//        int _disconnected = 0;
//        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs(); // 재사용 해야지 계속 생성하는건 말이 안되고 의미도 없어
//        Queue<byte[]> _sendQueue = new Queue<byte[]>();
//        bool _pending = false; // 한번이라도 했고 보내고 있는지를 확인 하기 위해
//        object _lock = new object();
//        public void Start(Socket socket)
//        {
//            _socket = socket;
//            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
//            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
//            recvArgs.SetBuffer(new byte[1024], 0, 1024);
//            //이렇게 하는 이유는 버퍼를 엄청 크게 나눠서 세션끼리 나눠쓰는 경우가 있기 때문
//            //이렇게 하면 세션을 만들때 마다 버퍼를 만듬
//            //추가 정보를 주고 싶으면
//            //recvArgs.UserToken = 어떤 정보든 상관 없음; 식별자나 연동하고 싶은 데이터가 있을대
//            //SetBuffer로 해줘야한다 기존에 리슨때는 args.AcceptSocket = null;
//            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
//            RegisterRecv(recvArgs);

//            //낚시대를 쓰고 올려서 다시 던지고하는것과 같다

//        }
//        //send는 async가 아직 아님 그리고 그렇다고 해도 문제가 있다.
//        /*public void Send(byte[] sendBuff)
//        {
//            //_socket.Send(sendBuff);
//            SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
//            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
//            sendArgs.SetBuffer(sendBuff,0,sendBuff.Length);
//            Registersend(sendArgs);
//            //MMO를 만들면 유저가 많은데 조금만 움직여도 모두에게 정보를 보내야 하는데
//            //이걸 계속한다는건 말이 안된다.
//            //데이터를 뭉쳐서 보내면 좋겠다.
//            //그리고 이전에 만든 SocketAsyncEventArgs를 재사용하는 방법으로
//        }*/

//        /*        public void Send(byte[] sendBuff)
//                {
//                    _sendArgs.SetBuffer(sendBuff, 0, sendBuff.Length);
//                    Registersend();
//                    //MMO를 만들면 유저가 많은데 조금만 움직여도 모두에게 정보를 보내야 하는데
//                    //이걸 계속한다는건 말이 안된다.
//                    //데이터를 뭉쳐서 보내면 좋겠다.
//                    //그리고 이전에 만든 SocketAsyncEventArgs를 재사용하는 방법으로
//                }
//                //이렇게 고친 방법은 더욱 멀티 쓰레드 환경에서 문제가 된다
//                //버퍼 전달이 안되도 버퍼를 맘대로 바꿀 수 있기 때문
//                //매번 RegisterSend로 보내는게 아닌 전송이 끝날때 까지 모든 데이터를 큐에 쌓아두고 완료되면 보내는 방식*/

//        public void Send(byte[] sendBuff)
//        {
//            lock (_lock) //락을 잡아 한번에 한명씩만
//            {
//                _sendQueue.Enqueue(sendBuff);
//                if (!_pending) Registersend();
//            }
//        }

//        public void Disconnect()
//        {
//            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
//                return;
//            _socket.Shutdown(SocketShutdown.Both);
//            _socket.Close();
//        }
//        //동시다발적 혹은 동시에 2개가 실행이 되면?

//        #region 네트워크
//        void Registersend() // 어차피 전역으로 선언되었기 때문에 매개변수로 넣어줄 필요가 없다
//        {
//            _pending = true;
//            byte[] buff = _sendQueue.Dequeue();
//            _sendArgs.SetBuffer(buff, 0, buff.Length);

//            bool pendig = _socket.SendAsync(_sendArgs);
//            if (!pendig) OnSendCompleted(null, _sendArgs);
//        }

//        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
//        {
//            lock(_lock)// 콜백 방식으로 다른 쓰레드에서 사용할 수 있기 때문에 lock을 사용
//            {
//                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
//                {

//                    //이제 애매한 부분 Send하는 부분이 정해져있지 않다
//                    //다른 방법으로 접근해야 한다
//                    try
//                    {
//                        //이전과 같이 하면 버퍼가 2개가 되버리는 문제가 발생한다
//                        //똑같은 정보를 2번 보내는 일
//                        if (_sendQueue.Count > 0)
//                        {
//                            Registersend();
//                        }
//                        else
//                         _pending = false;
                        
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine($"OnSendCompleted Failed {ex}");
//                    }
//                }
//                else
//                {
//                    // 상대방에게 문제가 있다는거니까
//                    Disconnect();
//                }
//            }
//        }
//        void RegisterRecv(SocketAsyncEventArgs args)
//        {
//            bool pending = _socket.ReceiveAsync(args);
//            if (!pending) OnRecvCompleted(null, args);
//        }
        
//        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
//        {
//            //이부분은 좀 다름
//            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success) // 몇 바이트를 받았느냐
//            {
//                //TODO
//                try
//                {
//                    string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred); // 어디서부터 시작하냐 Offset
//                    Console.WriteLine($"[From client]{recvData}");
//                    RegisterRecv(args);
//                }
//                catch(Exception e)
//                {
//                    Console.WriteLine(e.ToString());
//                }
//            }
//            else
//            {
//                Disconnect();
//            }
//        }
//    }
//    #endregion
//}
