using System;

namespace ServerCore.기초
{
    /*    class Program
        {
            static int x = 0;
            static int y = 0;
            static int r1 = 0;
            static int r2 = 0;

            //기존 문제되던 코드
            *//*static void Thread_1()
            {
                y = 1; // Store y
                r1 = x; // Load x
            }

            static void Thread_2()
            {
                x = 1; // Store x
                r2 = y; // Load y
            }*//*

            static void Thread_1()
            {
                y = 1; // Store y
                Thread.MemoryBarrier(); //마음대로 못바꾸게 선을 긋는것
                r1 = x; // Load x
            }

            static void Thread_2()
            {
                x = 1; // Store x
                Thread.MemoryBarrier();
                r2 = y; // Load y
            }

            static void Main(string[] args)
            {
                int count = 0;
                while (true)
                {
                    count++;
                    x = y = r1 = r2 = 0;
                    Task t1 = new Task(Thread_1);
                    Task t2 = new Task(Thread_2);
                    t1.Start();
                    t2.Start();

                    Task.WaitAll(t1, t2);

                    if (r1 == 0 && r2 == 0)
                        break;
                }
                Console.WriteLine($"{count}번만에 빠져나옴");
                *//*
                 잘 빠져나오고 있다 어떻게 이게 가능한걸까? 지금 코드만 보면 둘다 0이 되는 경우는 없다
                 하드웨어도 우리를 위해 최적화 해주고 있다
                CPU의 경우 명령어를 볼때 의존성이 없다면 자기 멋대로 뒤바꿀 수 있다.

                y = 1; // Store y
                r1 = x;
                이거를 뒤집어서 
                r1 = x;
                 y = 1; 
                이렇게 실행되는 경우가 발생된다는 소리 이러면 전부다 0이 나올 수 있다
                이게 어떻게 가능한가? 싶은데 싱글에서는 문제가 되는게 없다 멀티에서는 예상한 로직이 꼬이게 된다는 문제가 발생된다
                이때문에 사용한는것이 메모리 배리어다
                두가지 기능 1) 코드 재배치 억제 2) 가시성
                 Thread.MemoryBarrier();를 넣어주면 되는데 이를 넣으면 무한루프를 돌게 된다

                이는 한개만 있는게 아니라 종류별로 있다
                1) Full Memory Barrier (ASM MFENCE, C# Thread.MemoryBarrier) : Store/Load 둘다 막는다
                변수에 값을 넣는게 Store
                변수에서 값을 가져오는게 Load

                2) Store Memory Barrier (ASM SFENCE) : store만 막는다
                3) Load Memory Barrier (ASM LFENCE) : Load만 막는다
                2,3은 쓸일이 거의 없다

                가시성이란
                캐시에 저장된거를 다른 코어가 볼 수 있냐 없냐
                가시성을 보존하기 위해서 수첩에만 넣는게 아니라 주문 현황에 넣어야 한다
                직원이 주문현황에 수첩 내용을 넣고 직원2는 이를 가져와서 본인 정보도 최신화 해야한다 커밋을 통한 동기화 작업

                메모리 배리어는 이런 동기화 작업도 해준다 그렇기에 스토어를 하고 로드하기 전에 실행시켜줘야 한다
                이는 한개만 해주는게 아니라 받는 작업을 해줘야 하기 때문에 각 쓰레드마다 메모리 베리어를 실행한다

                Volatile도 일종의 배리어가 간접적으로 동작하고 있다고 생각하면 된다
                *//*
            }
        }*/

}