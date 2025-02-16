using DummyClient;
using ServerCore;
using System;
using System.Collections;
using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    ServerSession _session = new ServerSession();

    public void Send(ArraySegment<byte> sendBuff)
    {
        _session.Send(sendBuff);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string host = Dns.GetHostName(); // �� ���� ȣ��Ʈ�� ������
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);// �� ������ �Ĺ��� ��ȣ�� ��Ÿ��(�ּҿ� ��Ʈ)

        //�����ʸ� ����ߴ��� ó�� Ŀ��Ʈ�� ����Ѵ�
        Connector connector = new Connector();
        connector.Connect(endPoint,
            () =>
            {
                return _session;
            }, 1); //  N�� ����
    }

    // Update is called once per frame
    void Update()
    {
        System.Collections.Generic.List<IPacket> list = PacketQueue.Instance.PopAll();
        foreach (IPacket packet in list)
        {
            PacketManager.Instance.HandlePacket(_session, packet);
            
        }
    }

    void OnApplicationQuit() // ���� ���� X
    {
        if (_session != null)
        {
            Debug.Log("[Unity] ���� ���� - ���� ���� ����.");
            _session.Disconnect();
        }
    }
}
