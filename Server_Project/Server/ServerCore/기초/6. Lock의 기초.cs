using System;

namespace ServerCore.기초
{
    /*같은 변수를 다른 쓰레드에서 사용한다면 이걸 가져올때 문제가 된다는것을 저번에 확인했다
     크리티컬 섹션 - 인계영역
    이거를 해결하기 위한 방법으로 interlock을 걸었던것
    다만 이번에는 이를 사용하지 않고 선을 그어서 내가 쓸꺼니까 지금 다들 이 변수 쓰지마 하는 방법을 사용할 것

    Monitor은 문을 락거는 행위
    화장실을 갈때 누가 안에서 문을 잠그면 못들어간다 

    다시 문이 열릴때까지 대기한다
    상호배제 Mutual Exclusive
    이 enter와 exit 사이는 싱글 쓰레드라고 생각하고 작성해도 무관하다
    언어마다 이런건 있다
    c++은 CriticalSection std::mutex
    다만 이렇게 사용하면 관리하기가 어려워지고 
    exit를 안하고 return을 하게 된다면 또 문제가 발생된다 이런 상황을 데드락 상황이라고 한다 DeadLock
    일반적으로 사용하고 싶으면 try 문법을 사용해서 finally를 사용
    try
                    {
                        Monitor.Enter(_obj);
                        number++;
                        return;
                    }
                    finally { Monitor.Exit(_obj); }
    이런식으로 하면 무조건 exit가 실행된다.
    대부분은 lock 키워드를 사용 이는 위 try와 똑같은 역할 이를 편리하게 사용할 수 있음
     *//*
    class Program
    {
        static int number = 0;
        static object _obj = new object();
        static void Thread_1()
        {
            for (int i = 0; i < 10000000; i++)
            {

                *//*               Monitor.Enter(_obj); // 문을 잠그는 행위
                               number++;
                               Monitor.Exit(_obj); // 작업이 끝나서 문을 다시 열었다면*/
    /*try
    {
        Monitor.Enter(_obj);
        number++;
        return;
    }
    finally { Monitor.Exit(_obj); }*//*
    lock (_obj)
    {
        //이때 obj가 자물쇠의 역할을 새준다고 생각하면 된다 이를 빠져나올때 잠금을 풀어주는거
        number++;
    }
}
}

static void Thread_2()
{
for (int i = 0; i < 10000000; i++)
{
    *//*Monitor.Enter(_obj); // 문이 잠겨있으니 대기 > 문이 열렸다면 이제 들어가서 문을 잠군다
    number--;
    Monitor.Exit(_obj);*//*
    lock (_obj)
    {
        //이때 obj가 자물쇠의 역할을 새준다고 생각하면 된다 이를 빠져나올때 잠금을 풀어주는거
        number--;
    }
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

}*/
}