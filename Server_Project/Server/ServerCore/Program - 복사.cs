using System;

namespace ServerCore
{
    class SessionManager
    {
        static object _lock = new object();
        public static void TestSession()
        { 
            lock(_lock)
            {

            }
        }

        public static void Test()
        {
            lock(_lock)
            {
                UserManager.TestUser();
            }
        }
    }

    class UserManager
    {
    static object _lock = new object ();
        public static void Test()
        {
            lock(_lock)
            {
                SessionManager.TestSession();
            }
        }

        public static void TestUser()
        {
            lock(_lock)
            {

            }
        }
    }
    

    class Program
    {
        /*락이 걸려 접근을 못하는 경우는 코드를 잘못짠거
        고차원적인 데드락이 많다
        자물쇠가 2개가 있고 둘다 잠궈야지 화장실에 들어갈 수 있다고 가정하는데

        쓰레드 2개가 각각 한개씩 접근을 했을때
        그러면 두개를 동시에 접근하는 경우는 없을꺼다
        자물쇠를 먼저 접근한거 부터 잠그고 다른 자물쇠를 잠궈서 문을 완전이 잠그고 싶은데
        다른 자물쇠는 이미 다른 쓰레드가 접근중이다.
        이는 다른 쓰레드도 마찬가지인 상황이라 계속 잠글 수 없는 상황이 발생

        이를 해결하는 방법은 무조건 위쪽부터 잠그고 다음에 다음 자물쇠까지 잠그자는 약속을 넣는다

        하지만 이상하다 화장실은 한개인데 자물쇠는 2개라는 비유는 이상하지만 코드내에서는 발생 가능성 있다
        
        이는 Monitor로 해결
        Monitor에는 TryEnter라는게 존재 락을 얻는데 실패하면 나간다 이런 방법이 있지만 애초에 그러면 락에 문제가 있다는 것
        */

        static void Thread_1()
        {
            for (int i = 0; i < 10000000; i++)
            {
                SessionManager.Test(); 
            }
        }
        static void Thread_2()
        {
            for (int i = 0; i < 10000000; i++)
            {
                UserManager.Test();
            }
        }
        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            /*t1.Start();
            t2.Start();*/

            t1.Start();
            Thread.Sleep(100); // 강제로 시간을 어긋나게 하면 대드락 해결된다 (이런건 크러쉬가 일어나면 고치면 된다)
            //락을 걸기전에 각 클래스로 만들어 id를 준다 그리고 id 차이를 본다음 문제가 있는지 여부를 확인한다
            t2.Start();
            Task.WaitAll(t1, t2);
            
        }
    }

}