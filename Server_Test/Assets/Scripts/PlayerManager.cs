using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    MyPlayer _myPlayer;
    Dictionary<int, Player> _players = new Dictionary<int, Player>();

    public static PlayerManager Instance { get; } = new PlayerManager();

    public void Add(S_PlayerList packet)
    {
        GameObject obj = Resources.Load<GameObject>("Player");
        foreach (S_PlayerList.Player p in packet.players)
        {
            GameObject go = Instantiate(obj) as GameObject;

            if(p.isSelf)
            {
                MyPlayer my = go.AddComponent<MyPlayer>();
                my.playerId = p.playerId;
                my.transform.position = new Vector3(p.posX, p.posY, p.posZ);
                _myPlayer = my;
            }
            else
            {
                Player player = go.AddComponent<Player>();
                player.playerId = p.playerId;
                player.transform.position = new Vector3(p.posX, p.posY, p.posZ);
                _players.Add(p.playerId, player);
            }
        }
    }

    public void EnterGame(S_BroadcastEnterGame pkt)
    {
        if (pkt.playerId == _myPlayer.playerId)
            return;
        GameObject obj = Resources.Load<GameObject>("Player");
        GameObject go = Instantiate(obj) as GameObject;
        Player player = go.AddComponent<Player>();
        player.transform.position = new Vector3(pkt.posX, pkt.posY, pkt.posZ);
        _players.Add(pkt.playerId, player);
    }

    public void LeaveGame(S_BroadcastLeaveGame pkt)
    {
        if(_myPlayer.playerId == pkt.playerId)
        {
            GameObject.Destroy(_myPlayer.gameObject);
            _myPlayer = null;
        }
        else
        {
            Player player = null;
            if(_players.TryGetValue(player.playerId, out player))
            {
                GameObject.Destroy(player.gameObject);
                _players.Remove(player.playerId);
            }
        }
    }

    public void Move(S_BroadcastMove pkt)
    {
        //움직이는건 까다롭다
        //방법은 2가지 패킷이 올때마다 이동시키는 방법과
        //클라이언트에서 예측해서 움직이다가 패킷이 오면 보정하는 방식
        //일단 여기서는 패킷이 오면 이동하는 방법으로 진행
        if(_myPlayer.playerId == pkt.playerId)
        {
            _myPlayer.transform.position = new Vector3(pkt.posX, pkt.posY, pkt.posZ);
        }
        else
        {
            Player player = null;
            if(_players.TryGetValue(pkt.playerId, out player))
            {
                player.transform.position = new Vector3(pkt.posX, pkt.posY, pkt.posZ);
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
