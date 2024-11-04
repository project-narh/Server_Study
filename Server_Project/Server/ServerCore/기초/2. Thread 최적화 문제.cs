using System;

namespace ServerCore.기초
{
    /*class Program
    {
        volatile static bool _stop = false; // 모든 쓰레드가 동시 접근 가능 Static 이니까
        //static bool _stop = false;
        static void ThreadMain()
        {
            Console.WriteLine("쓰레드 시작");

            while (!_stop)
            {
                
            }
            *//*디버그 > 창 > 디스어셈블리 
             * 이를 어셈블리로 확인하면

            if(_stop == false)
            {
                while(true)
                {

                }
            }
            이러한 형태로 되어있다
            컴파일 과정에서 반복문 내부에 _stop을 바꾸는 명령어가 없어서 맘대로 이렇게 수정한 것(멀티쓰레드 개념을 컴파일러가 모르기에)
            이를 최적화 하지 않게 하는 방법은 앞에 volatile를 붙여준다 : volatile static bool _stop = false; (c++에서는 의미가 다르다 캐시를 무시하고 최신 값을 가져와라 라는 뜻도 있음)
            c# 전문가들은 쓰지 말라고 하는 방법이니 이런애가 있구나 정도로만 알고 넘어가자
             *//*

            Console.WriteLine("쓰레드 종료");
        }

        static void Main(string[] args)
        {
            Task t = new Task(ThreadMain);
            t.Start();

            Thread.Sleep(1000);// 1초동안 쓰레드 sleep
            _stop = true;
            Console.WriteLine("STOP 호출");
            Console.WriteLine("종료 대기중");
            t.Wait();//Thread의 join과 동일 쓰레드가 종료되길 기다리는 함수
            Console.WriteLine("종료 성공");

            //평소에는 Debug 모드지만 배포에는 Release를 사용한다(최적화가 자동 진행) 이를 사용하니 위 함수가 무한루프에 빠지게 된다
            // 멀티 쓰레드를 사용하면 이런 상황이 많이 사용된다 멀티쓰레드는 흑마법
            // 디버그 > 창 > 디스어셈블리

        }
    }*/

}