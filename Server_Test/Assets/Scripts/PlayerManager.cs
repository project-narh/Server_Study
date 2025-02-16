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
        //�����̴°� ��ٷӴ�
        //����� 2���� ��Ŷ�� �ö����� �̵���Ű�� �����
        //Ŭ���̾�Ʈ���� �����ؼ� �����̴ٰ� ��Ŷ�� ���� �����ϴ� ���
        //�ϴ� ���⼭�� ��Ŷ�� ���� �̵��ϴ� ������� ����
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
