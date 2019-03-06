using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlacingAreaCheck : ScriptableObject
{
    public abstract bool CheckPlaceable(Vector2 position, float rotation);

    public abstract void VisualizeArea(bool on);
}
