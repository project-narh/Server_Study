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
        StartCoroutine("CoSendPacket");
    }

    // Update is called once per frame
    void Update()
    {
        //����� �� �����ӿ� �Ѱ��� ó���ϰ� �־ while �ݺ��� ���� �ð� ������ �δ� �ϸ� �ȴ�
        IPacket packet = PacketQueue.Instance.Pop();
        if(packet != null)
        {
            Debug.Log("ó���� ��Ŷ ����");
            PacketManager.Instance.HandlePacket(_session, packet); // ���� �Ȱ��� �ڵ鷯���� �۾�������
            //�Ʊ�� �ٸ��� ����Ƽ ���ξ����忡�� ����ȴ�
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

    IEnumerator CoSendPacket()
    {
        Debug.Log("����");
        while (_session != null)
        {
            yield return new WaitForSeconds(3f);
            C_Chat chatPacket = new C_Chat();
            chatPacket.chat = "Hello Unity!!";
            ArraySegment<byte> segment = chatPacket.Write();
            if(_session != null)
            {
                Debug.Log("[CoSendPacket] ���� �õ�...");
                _session.Send(segment);
                Debug.Log("[CoSendPacket] ���۳Ѿ��");
            }
            else
            {
                Debug.Log("���� �߻�");
            }
        }
    }
}
