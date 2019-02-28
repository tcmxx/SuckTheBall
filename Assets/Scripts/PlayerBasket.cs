using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasket : MonoBehaviour
{
    public string targetBallTag = "TargetBall";
    public int playerIndex;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(targetBallTag))
        {
            //Destroy(collision.gameObject);
            Photon.Pun.PhotonNetwork.Destroy(collision.gameObject);
            GamePlayController.Instance.PlayerGetPoint(playerIndex);
        }
    }
}
