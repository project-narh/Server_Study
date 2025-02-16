using ServerCore;
using System;
using System.Collections;
using UnityEngine;

public class MyPlayer : Player
{
    public NetworkManager NetworkManager;

    //���� �����ϴ� �÷��̾� ���� �����ϴ¾ָ� ���콺 �Է� �޾ƾ� �Ѵ�
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        NetworkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        StartCoroutine("CoSendPacket");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CoSendPacket()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);
            C_Move movePacket = new C_Move();
            movePacket.posX = UnityEngine.Random.Range(-50, 50);
            movePacket.posY = 0;
            movePacket.posZ = UnityEngine.Random.Range(-50, 50);
            NetworkManager.Send(movePacket.Write());
        }
    }
}
