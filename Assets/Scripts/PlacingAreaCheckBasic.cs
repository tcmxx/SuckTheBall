using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PlacingAreaCheckBasic : PlacingAreaCheck
{
    public GameObject noPlacingPrefab;
    public LayerMask noPlacingLayer;

    protected GameObject noPlacingInstance = null;

    protected bool isVisualOn = false;

    public override bool CheckPlaceable(Vector2 position, float rotation)
    {
        if (noPlacingInstance == null)
            InstantiateNoPlacingInstance();
        var hit = Physics2D.Raycast(position, Vector2.zero, 0, noPlacingLayer);
        return hit.collider == null;

    }

    public override void VisualizeArea(bool on)
    {
        if (isVisualOn == on)
            return;
        if (noPlacingInstance == null)
            InstantiateNoPlacingInstance();

        var sprites = noPlacingInstance.GetComponentsInChildren<SpriteRenderer>(true);
        foreach(var s in sprites)
        {
            s.enabled = on;
        }
        isVisualOn = on;
    }

    protected void InstantiateNoPlacingInstance()
    {
        noPlacingInstance = Instantiate(noPlacingPrefab);
        noPlacingInstance.transform.position = Vector3.zero;
        noPlacingInstance.transform.rotation = Quaternion.identity;
        noPlacingInstance.transform.localScale = Vector3.one;

        var sprites = noPlacingInstance.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var s in sprites)
        {
            s.enabled = isVisualOn;
        }
    }
}
