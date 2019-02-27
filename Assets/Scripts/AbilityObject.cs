using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AbilityObject : MonoBehaviour
{

    public GameObject visualGhostNotPlaceable;
    public GameObject visualGhostPlaceable;
    public GameObject visualPlaced;
    public GameObject effectedGameObject;

    public Status CurrentStatus { get { return mStatus; } protected set { mStatus = value; } }
    [SerializeField]
    protected Status mStatus = Status.GhostNotPlaceable;

    public double EffectTime { get;  set; }
    protected int playerIndex;
    protected int PlayerIndex { get { return playerIndex; } }


    public enum Status
    {
        GhostNotPlaceable,
        GhostPlaceable,
        Placed,
        Effected
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(CurrentStatus == Status.Placed)
        {
            if(PhotonNetwork.Time >= EffectTime)
            {
                SetStatus(Status.Effected);
            }
        }
    }



    public virtual bool CheckPlaceable()
    {
        return true;
    }
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



    public virtual void SetPlayer(int index)
    {
        playerIndex = index;
    }
}
