using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public struct PlayerInGameInfo
{
    public int score;
    public float mana;
}


public class GamePlayController : MonoBehaviour,IPunObservable
{
    public int PlayerIndex { get; }
    public static GamePlayController Instance { get; protected set; }

    public float startDelay = 3;

    public PlayerInGameInfo Player0Info { get; private set; }
    public PlayerInGameInfo Player1Info { get; private set; }

    public NetworkObjectPool prefabPool;
    private void Awake()
    {
        Instance = this;
        Photon.Pun.PhotonNetwork.PrefabPool = prefabPool;
    }
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(startDelay);
        TargetBallSpawner.Instance.Started = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool PlayerManaEnough(int playerIndex, float manaNeeded)
    {
        if (playerIndex == 0)
        {
            return Player0Info.mana >= manaNeeded;
        }
        else
        {
            return Player1Info.mana >= manaNeeded;
        }
    }

    public void CostPlayerMana(int playerIndex, float cost)
    {
        if (playerIndex == 0)
        {
            var temp = Player0Info;
            temp.mana -= cost;
            Player0Info = temp;
        }
        else
        {
            var temp = Player1Info;
            temp.mana -= cost;
            Player1Info = temp;
        }
    }

    public void PlayerGetPoint(int playerIndex)
    {
        if(playerIndex == 0)
        {
            var temp = Player0Info;
            temp.score++;
            Player0Info = temp;
        }
        else
        {
            var temp = Player1Info;
            temp.score++;
            Player1Info = temp;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(Player0Info);
            stream.SendNext(Player1Info);
        }
        else
        {
            // Network player, receive data
            this.Player0Info = (PlayerInGameInfo)stream.ReceiveNext();
            this.Player1Info = (PlayerInGameInfo)stream.ReceiveNext();
        }
    }
}
