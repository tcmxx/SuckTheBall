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
    public float maxLifetime = 999;
    public bool allowRotation = true;
    public string abilityPrefabID;
    public AbilityCommand GenerateCommand(Vector2 position, float rotation, int playerIndex)
    {
        AbilityCommand command = new AbilityCommand();
        command.abilityID = abilityID;
        double delay = PhotonNetwork.IsMasterClient ? 0 : NetworkController.Instance.interpolationBackTime;
        //command.effectTime = PhotonNetwork.Time + effectDelay - delay;
        command.playerIndex = playerIndex;
        command.positionX = position.x;
        command.positionY = position.y;
        command.rotation = rotation;
        command.useTime = PhotonNetwork.Time - delay;
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
