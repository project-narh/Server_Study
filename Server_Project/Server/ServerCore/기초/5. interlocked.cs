using System;

namespace ServerCore.기초
{
    /*class Program
    {*//*
      가시성 문제는 아니고 이렇게 하면 0이 나와야 하는거 아닌가 왜 값이 다르지? 사실 난 지금 0이 나오는 중이긴 함
        
        Race Condition : 경합 조건에 대한 이야기다
        식당이 잘되서 신입 직원 3명을 채용 주문 생기면 바로 처리하려고 다 대기중인 상태
        보자마자 직원 3명이 바로 처리해서 3개가 배달옴 (하지만 1개만 시킴 이런 상황을 레이스 컨디션 즉 경합 조건이라고 말한다)
        동시 다발적으로 일이 발생

        어셈블리는 오른쪽에서 왼쪽으로 읽으면 된다
        number++을 어셈블리로 보면
        메모리값을 ecx로 옮기고 1을 더하고 다시 기존 메모리값으로 옮긴다
        즉
        number++;는
        int temp = number;
        temp += 1;
        number = temp; 와 동일하다
        이래서 문제가 되는거다
        number이 tmep에 저장되고 다시 돌려받는 과정에서 이것마저 값을 공유하는게 아니라서
        위에서 number = temp(1) 과정에서 아래는 temp -= 1 또는 number = temp(-1) 이 이뤄지고 있고
        그로인하여 오차가 발생

        atomic = 원자성 이는 DB에서도 나오는 문제
        상점에서 아이템을 구매할때 골드를 빼주고 인벤에 아이템을 추가 하면서 DB에 저장하는데 
        골ㄷ를 빼고 서버가 다운이 된다면 골드만 빠지고 아이템은 안 생기는 문제 발생
        아이템 교환에서도
        전달되는 과정에서 이와 같은 문제가 발생하여 아이템 복사가 일어날 수 있다.

        이럴땐 Interlocked를 사용한다 이는 원자적으로 이뤄지는걸 보장 (더이상 쪼갤 수 없는)
        다만 성능에서 문제가 발생되긴 한다.
        number++로 3번의 작업이 아닌 한번만 이뤄진다
        *//*
        static int number = 0;
        static void Thread_1()
        {
            for (int i = 0; i < 10000000; i++)
            {
                //number++;
                Interlocked.Increment(ref number); //원자적 덧셈
                //All or Nothing 실행이되거나 안되거나
                //이를 위해 기존 연산이 끝나야 연산하고 다른 연산은 잠시 멈춘다
                //동시에 실행되는게 아니라 실행순서가 생기게 되고 이로인해 결과를 보장받는다.
                //다만 이러면 좀 더 느려지긴한다.
                *//*
                number++;
                는
                int temp = number;
                temp += 1;
                number = temp; 와 동일하다
                *//*
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 10000000; i++)
            {
                //number--;
                Interlocked.Decrement(ref number); // 원자적 뺄셈
            }
        }

        static void Main(string[] args)
        {

            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();
            t2.Start();

            Console.WriteLine(number);
        }
    }
*/
}