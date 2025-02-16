using ServerCore;
using System;
using System.Collections;
using UnityEngine;

public class MyPlayer : Player
{
    public NetworkManager NetworkManager;

    //내가 조작하는 플레이어 내가 조종하는애만 마우스 입력 받아야 한다
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
