using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ExitGames.Client.Photon;


[System.Serializable]
public struct AbilityCommand
{
    public int playerIndex;
    public string abilityID;
    public double useTime;
    public double effectTime;
    public float rotation;
    public float positionX;
    public float positionY;
    // Convert an object to a byte array
    public static byte[] ObjectToByteArray(object obj)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (var ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    // Convert a byte array to an Object
    public static object ByteArrayToObject(byte[] arrBytes)
    {
        using (var memStream = new MemoryStream())
        {
            var binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = binForm.Deserialize(memStream);
            return obj;
        }
    }
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
        PhotonPeer.RegisterType(typeof(AbilityCommand), (byte)'C', AbilityCommand.ObjectToByteArray, AbilityCommand.ByteArrayToObject);
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

    public AbilityInfo GetAbilityInfoByID(string abilityID)
    {
        AbilityInfo abilityInfo = null;
        gameDataRegister.AbilityRegister.TryGetValue(abilityID, out abilityInfo);
        return abilityInfo;
    }

    [PunRPC]
    public void ExecuteCommand(AbilityCommand command)
    {
        AbilityInfo abilityInfo = null;
        if (gameDataRegister.AbilityRegister.TryGetValue(command.abilityID, out abilityInfo))
        {
            GamePlayController.Instance.CostPlayerMana(command.playerIndex, abilityInfo.manaCost);
            GameObject temp = PhotonNetwork.InstantiateSceneObject(abilityInfo.abilityPrefabID, new Vector2(command.positionX,command.positionY), Quaternion.Euler(0, 0, command.rotation));
            AbilityObject abilityObj = temp.GetComponent<AbilityObject>();

            abilityObj.BroadcastSetPlayer(command.playerIndex);
            abilityObj.BroadcastSetStatus(AbilityObject.Status.Placed);
            abilityObj.EffectTime = command.effectTime;
            
        }
        else
        {
            Debug.LogError("Unregistered ability ID:" + command.abilityID);
        }
    }
}
