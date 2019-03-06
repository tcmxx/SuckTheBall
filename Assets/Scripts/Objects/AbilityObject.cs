using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;

public class AbilityObject : MonoBehaviourPun
{

    public GameObject visualGhostNotPlaceable;
    public GameObject visualGhostPlaceable;
    public GameObject visualPlaced;
    public GameObject effectedGameObject;
    public PlacingAreaCheck placingCheck;

    public UnityEvent onDestroy;

    public Status CurrentStatus { get { return mStatus; } protected set { mStatus = value; } }
    [SerializeField]
    protected Status mStatus = Status.GhostNotPlaceable;

    public double EffectTime { get;  set; }
    public double DestroyTime { get; set; }
    protected int playerIndex;
    protected int PlayerIndex { get { return playerIndex; } }
    
    public enum Status
    {
        GhostNotPlaceable,
        GhostPlaceable,
        Placed,
        Effected
    }


    // Update is called once per frame
    protected virtual void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (CurrentStatus == Status.Placed)
            {
                if (PhotonNetwork.Time >= EffectTime)
                {
                    BroadcastSetStatus(Status.Effected);
                }

                if (PhotonNetwork.Time >= DestroyTime)
                {
                    NetworkDestroy();
                }
            }
        }
    }


    public virtual bool CheckPlaceable()
    {
        return placingCheck.CheckPlaceable(transform.position, transform.rotation.eulerAngles.z);
    }
    public void ShowPlacingArea(bool show)
    {
        placingCheck.VisualizeArea(show);
    }

    public void BroadcastSetStatus(Status status)
    {
        if (PhotonNetwork.OfflineMode)
            SetStatus(status);
        else
            photonView.RPC("SetStatus", RpcTarget.All, status);
    }

    public void BroadcastSetPlayer(int index)
    {
        if (PhotonNetwork.OfflineMode)
            SetPlayer(index);
        else
            photonView.RPC("SetPlayer", RpcTarget.All, index);
    }
    [PunRPC]
    public virtual void SetStatus(Status status)
    {
        CurrentStatus = status;
        if (status == Status.Effected)
        {
            effectedGameObject.SetActive(true);
            visualGhostNotPlaceable.SetActive(false);
            visualGhostPlaceable.SetActive(false);
            visualPlaced.SetActive(false);
        }else if(status == Status.GhostNotPlaceable)
        {
            effectedGameObject.SetActive(false);
            visualGhostNotPlaceable.SetActive(true);
            visualGhostPlaceable.SetActive(false);
            visualPlaced.SetActive(false);
        }
        else if (status == Status.GhostPlaceable)
        {
            effectedGameObject.SetActive(false);
            visualGhostNotPlaceable.SetActive(false);
            visualGhostPlaceable.SetActive(true);
            visualPlaced.SetActive(false);
        }
        else if (status == Status.Placed)
        {
            effectedGameObject.SetActive(false);
            visualGhostNotPlaceable.SetActive(false);
            visualGhostPlaceable.SetActive(true);
            visualPlaced.SetActive(false);
        }
    }
    [PunRPC]
    public virtual void SetPlayer(int index)
    {
        playerIndex = index;
        var colors = GetComponentsInChildren<PlayerColorSetter>(true);
        foreach(var c in colors)
        {
            c.SetColorPlayer(PlayerIndex);
        }
    }

    public void NetworkDestroy()
    {
        onDestroy.Invoke();
        PhotonNetwork.Destroy(gameObject);  //for now just destroy it directly using photon. Probably should delay the client's destroy
    }
}
