using DummyClient;
using ServerCore;
using System;
using System.Collections;
using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    ServerSession _session = new ServerSession();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string host = Dns.GetHostName(); // 내 로컬 호스트의 도메인
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);// 각 정문과 후문의 번호를 나타냄(주소와 포트)

        //리스너를 사용했던거 처럼 커넥트를 사용한다
        Connector connector = new Connector();
        connector.Connect(endPoint,
            () =>
            {
                return _session;
            }, 1); //  N개 생성
        StartCoroutine("CoSendPacket");
    }

    // Update is called once per frame
    void Update()
    {
        //현재는 한 프레임에 한개씩 처리하고 있어서 while 반복을 쓰던 시간 제한을 두던 하면 된다
        IPacket packet = PacketQueue.Instance.Pop();
        if(packet != null)
        {
            Debug.Log("처리할 패킷 있음");
            PacketManager.Instance.HandlePacket(_session, packet); // 이제 똑같이 핸들러에서 작업하지만
            //아까와 다르게 유니티 메인쓰레드에서 실행된다
        }

    }

    void OnApplicationQuit() // 강의 내용 X
    {
        if (_session != null)
        {
            Debug.Log("[Unity] 게임 종료 - 세션 연결 해제.");
            _session.Disconnect();
        }
    }

    IEnumerator CoSendPacket()
    {
        Debug.Log("실행");
        while (_session != null)
        {
            yield return new WaitForSeconds(3f);
            C_Chat chatPacket = new C_Chat();
            chatPacket.chat = "Hello Unity!!";
            ArraySegment<byte> segment = chatPacket.Write();
            if(_session != null)
            {
                Debug.Log("[CoSendPacket] 전송 시도...");
                _session.Send(segment);
                Debug.Log("[CoSendPacket] 전송넘어옴");
            }
            else
            {
                Debug.Log("문제 발생");
            }
        }
    }
}
