using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public struct AbilityCommand
{
    public int playerIndex;
    public string abilityID;
    public double useTime;
    public double effectTime;
    public float rotation;
    public Vector2 position;
}


public class CommandController : MonoBehaviourPun
{

    public static CommandController Instance { get; private set; }
    public GameDataRegister gameDataRegister;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void BroadcastCommand(AbilityCommand command)
    {
        if (PhotonNetwork.OfflineMode)
            ExecuteCommand(command);
        else
            photonView.RPC("ExecuteCommand", RpcTarget.MasterClient, command);
    }


    [PunRPC]
    public void ExecuteCommand(AbilityCommand command)
    {
        AbilityInfo abilityInfo = null;
        if (gameDataRegister.AbilityRegister.TryGetValue(command.abilityID, out abilityInfo))
        {
            GamePlayController.Instance.CostPlayerMana(command.playerIndex, abilityInfo.manaCost);
            GameObject temp = PhotonNetwork.Instantiate(abilityInfo.abilityPrefabID, command.position, Quaternion.Euler(0, 0, command.rotation));
            AbilityObject abilityObj = temp.GetComponent<AbilityObject>();

            abilityObj.SetPlayer(command.playerIndex);
            abilityObj.SetStatus(AbilityObject.Status.Placed);
            abilityObj.EffectTime = command.effectTime;
            
        }
        else
        {
            Debug.LogError("Unregistered ability ID:" + command.abilityID);
        }
    }
}
