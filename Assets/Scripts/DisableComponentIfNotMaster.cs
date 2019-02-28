using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DisableComponentIfNotMaster : MonoBehaviour
{

    public Rigidbody2D[] bodies;
    public Collider2D[] colliders;
    public MonoBehaviour[] behaviours;

    void Start()
    {
        if(!PhotonNetwork.IsMasterClient&& !PhotonNetwork.OfflineMode)
        {
            foreach(var c in bodies)
            {
                c.simulated = false;
            }

            foreach(var c in colliders)
            {
                c.enabled = false;
            }
            foreach(var c in behaviours)
            {
                c.enabled = false;
            }
        }
    }
    
}
