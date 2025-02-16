using DummyClient;
using ServerCore;
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
