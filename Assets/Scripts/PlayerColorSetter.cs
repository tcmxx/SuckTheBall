using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColorSetter : MonoBehaviour
{
    public Color player0Color;
    public Color player1Color;

    public SpriteRenderer spriteRenderer;


    public void SetColorPlayer(int playerIndex)
    {
        if (spriteRenderer != null)
            if (playerIndex == 0)
            {
                spriteRenderer.color = player0Color;
            }
            else
            {
                spriteRenderer.color = player1Color;
            }
    }

}
