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
//        List<ArraySegment<byte>> _Pendinglist = new List<ArraySegment<byte>>(); // 말그대로 대기중인 목록
//        Queue<byte[]> _sendQueue = new Queue<byte[]>();
//        object _lock = new object();
//        public void Start(Socket socket)
//        {
//            _socket = socket;
//            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs(); // _sendArgs처럼 빼도 무관 다만 C++서버는 이런식
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
//                //즉 리스트를 따로 관리하고 있으니 이것도 이렇게 리스트로 확인이 가능
//                if (_Pendinglist.Count == 0 ) Registersend(); // 이대로 사용해도 무관하나 좀 더 최적화하기 위해선 해당 메소드를 건드려야한다 

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
//        // 이대로 사용해도 무관하나 좀 더 최적화하기 위해선 해당 메소드를 건드려야한다 
//        void Registersend() // 어차피 전역으로 선언되었기 때문에 매개변수로 넣어줄 필요가 없다
//        {
//            /*_pending = true;
//            byte[] buff = _sendQueue.Dequeue();
//            _sendArgs.SetBuffer(buff, 0, buff.Length);*/
//            //args를 확인하면 버퍼 리스트가 존재 리스트에 연결된 모든 애들을 한번에 다 보내주는 기능을 한다
//            //즉 리스트를 만들어 사용하면 더 효율적
//            //조심할 점으로는 SetBuffer와 BufferList를 동시에 사용하면 에러 즉 한개만 선태 

//            while (_sendQueue.Count > 0)
//            {
//                byte[] buff = _sendQueue.Dequeue();
//                _Pendinglist.Add(new ArraySegment<byte>(buff, 0, buff.Length));
//                //_sendArgs.BufferList.Add(buff, 0, buff.Length); 이렇게 넣으면 안된다
//                //_sendArgs.BufferList.Add(new ArraySegment<byte>(buff, 0, buff.Length));
//                //byte[] 배열이 아닌 Arraysement를 원하기 때문
//                //따라 들어가면 Struct 형태 즉 힙이 아닌 스택에 데이터 저장
//                //이는 즉 할당이 아닌 복제형태로 들어갈 것
//                //C#은 배열을 볼때 C++은 포인터로 주소를 주면 되지만 C#은 그게 불가능
//                //무조건 버퍼의 첫 주소만 알 수 있어서 인덱스를 넘겨줘서 시작과 사용할 크기를 알려주는 것

//                //MSDN에도 안나왔지만 버퍼 리스트는 완성을 따로 만들어서 넘겨줘야 한다 그래야 문제가 안생김
//                //즉 _sendArgs.BufferList.Add(new ArraySegment<byte>(buff, 0, buff.Length)); 이렇게 추가하면 문제 생김
//                //위 처럼list.Add(new ArraySegment<byte>(buff, 0,buff.Length)); 이렇게 하고 완성본 넘겨야함


//                //이렇게 너무 한번에 보내버리면 안된다 버퍼를 추적해서 상대방이 받을 수 있는지 양이 많은지도 확인해야함
//                //상대방이 못받는데 너무 어거지로 보내버리면 안되기 때문
//                //너무 양이 이상하게 많이 오고(DDOS)하면 차단 하는 기능이 아직 없다

//                //프로젝트 마다 다르지만 엔진은 그냥 서로 보내기를 하다 컨텐츠를 만들대 모든 행위를 기록을 하고 한번에 보내는게 좋다고 생각한다고 말씀
//            }
//            _sendArgs.BufferList = _Pendinglist;


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
//                        //예약한 리스트가 완료되었을때니
//                        _sendArgs.BufferList = null;
//                        _Pendinglist.Clear();

//                        Console.WriteLine($"Transferred byte {_sendArgs.BytesTransferred}");

//                        //이전과 같이 하면 버퍼가 2개가 되버리는 문제가 발생한다
//                        //똑같은 정보를 2번 보내는 일
//                        if (_sendQueue.Count > 0)
//                        {
//                            Registersend();
//                        }
     
                        
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
