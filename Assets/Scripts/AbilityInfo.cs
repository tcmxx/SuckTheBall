using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
[CreateAssetMenu()]
public class AbilityInfo : ScriptableObject
{
    public string abilityID;
    public Sprite abilityButtonSprite;
    public int manaCost;
    public float effectDelay = 1;
    public bool allowRotation = true;
    public string abilityPrefabID;

    public AbilityCommand GenerateCommand(Vector2 position, float rotation, int playerIndex)
    {
        AbilityCommand command = new AbilityCommand();
        command.abilityID = abilityID;
        command.effectTime = PhotonNetwork.Time + effectDelay;
        command.playerIndex = playerIndex;
        command.position = position;
        command.rotation = rotation;
        command.useTime = PhotonNetwork.Time;
        return command;
    }

    public GameObject InstantiateForLocalIndicator(Vector2 pos, float angle)
    {
        return( (IPunPrefabPool)GamePlayController.Instance.prefabPool).Instantiate(abilityPrefabID, pos, Quaternion.Euler(0, 0, angle));
    }

    public void DestroyLocalIndicator(GameObject obj)
    {
       ((IPunPrefabPool)GamePlayController.Instance.prefabPool).Destroy(obj);
    }
}
