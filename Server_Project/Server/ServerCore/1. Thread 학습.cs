using System;

namespace ServerCore
{
    /*class Program
    {
        //쓰레드의 사용법 함수
        *//*static void MainThread()
        {
            while (true)
            Console.WriteLine("Hello Thread!");
            //쓰레드의 종료 시점은 함수가 종료될때
            //C#에선 쓰레드가 기본적으로 forground로 만들어진다(c++과 다른 점)
            //Back
        }*//*
        static void MainThread(object state)
        {
            for(int i = 0; i < 5; i++)
            Console.WriteLine("Hello Thread!");
            //쓰레드의 종료 시점은 함수가 종료될때
            //C#에선 쓰레드가 기본적으로 forground로 만들어진다(c++과 다른 점)
            //Back
        }
    static void Main(string[] args)
        {
            //쓰레드의 사용법
            *//* Thread t = new Thread(MainThread);//이 안에 넣을 일종의 메인 함수를 만들어야 한다
               t.Name = "Test Thread";
               t.IsBackground = true;
               t.Start();// 쓰레드 실행
               Console.WriteLine("waiting for  Thread");
               t.Join();// 해당 쓰레드가 끝날때까지 기다리겠다 (비동기)
               Console.WriteLine("Hello Main!");*/

            /*        
            이렇게 쓰레드를 만들어주는건 부담되는 일(정직원을 한명 추가한 것)
            경우에 따라 간단하게 알바형식으로 잠시만 쓰고 끝내는 방법이 존재한다
            금방 끝나는 작업을 쓰레드에 할당하는 건 너무 비효율적
            이를 위해 단기 알바바가 필요
            c#은 쓰레드 풀이 존재
            이에 어떤 일을 줄건지 몇명을 고용할건지를 설정 가능
            */

/*            Task t = new Task(() => { while (true) { } }, TaskCreationOptions.LongRunning); // 직원을 고용한다기 보단 직원이 할 일을 정의해서 사용하는 것에 가깝다.
            t.Start(); // 쓰레드랑 다른건 쓰레드 풀에 넣어서 관리한다 (옵션을 넣을 수 있다)
            //TaskCreationOptions.LongRunning 오래 걸리는 작업이라는걸 알려줌
*//*

            ThreadPool.SetMinThreads(1, 1); //최소는 1개
            ThreadPool.SetMaxThreads(5, 5); // 최대는 5개

            for(int i = 0; i < 5; i++)
            {
                //ThreadPool.QueueUserWorkItem((obj) => { while (true) { } }); // 최대 스레드 풀을 다 무한반복하여 사용하니 아래 스레드는 실행이 안됨
                Task t = new Task(() => { while (true) { } }, TaskCreationOptions.LongRunning);
                t.Start();
                //아주 오래 걸리는 건 이렇게 처리해주는게 좋다 TaskCreationOptions.LongRunning이걸 취소하면 이전과 동일하게 먹통이 될 것
                //그래서 쓰레드를 직접 다루는 일은 거의 없고 오래 걸리는건 Task로 만들어서 실행해도 충분하다

            }

            ThreadPool.QueueUserWorkItem(MainThread);
            *//*쓰레드 풀은 어떤 원리인가?
            New 쓰레드는 새로운 정직원으로 정식 고용
            쓰레드 풀은 이미 고용되서 대기중인 쓰레드를 사용한다는 차이(유동적인 차이)
            오프젝트 풀링이랑 비슷한 원리
            쓰레드 생성과 풀의 가장 큰 차이점은 스레드는 갯수를 지정하지 않아서 천개를 만들어도 잘 실행된다 (쓰레드 천 개 생성)
            그렇다고 효율이 좋아지는건 아니다 아무리 많이 생성해도 CPU 코어에 따라서 달라지게 된다
            그래서 CPU 코어를 맞춰주는게 중요
            이러면 코어가 다른 쓰레드로 이동하는게 더 큰 문제 발생
            
            풀은 최대가 정해져 있어서 천개를 실행해도 바로 다 실행되는게 아니라 다른게 끝나야 재투입한다
            다만 너무 오래걸리고 안끝나게 해놨다면 인력이 부족한 현상이 생길 수 있다 그러면 실행이 안되겠지 그러면 풀관련이 먹통이 된다.
            */

            /*
            단점을 극복하는 방법은 Task 이용
            *//*
            
            while (true)
            {

            }
        }
    }*/
}