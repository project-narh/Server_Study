using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    //해당 클래스는 잡큐를 이런 방식으로도 사용할 수있다(프로젝트 마다)를 보여주기 위한거(현재 프로젝트에서 사용은 X)

    interface ITask
    {
        void Execute();
    }
    //실행이 필요한거를 수동으로 만들어준다 (너무 불편하다는 단점이 존재 2010년도 이전에는 이렇게 만들었음 람다가 생긴지 얼마 안됨)
    //다만 람다를 쓰면 생기는 문제점도 있다
    class BroadcastTask : ITask
    {
        GameRoom _room;
        ClientSession _session;
        string _chat;

        BroadcastTask(GameRoom room, ClientSession session, string chat)
        {
            _room = room;
            _session = session;
            _chat = chat;
        }

        public void Execute()
        {
           // _room.Broadcast(_session, _chat);
        }
    }

    internal class TaskQueue
    {
        Queue<ITask> _queue = new Queue<ITask>();
    }
}
