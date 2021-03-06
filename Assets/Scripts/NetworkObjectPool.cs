﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class NetworkObjectPool : ScriptableObject, IPunPrefabPool
{

    public DicStringGameobject IDGameObjectPool;

    

    public void Destroy(GameObject gameObject)
    {
        GameObject.Destroy(gameObject);
    }

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        GameObject objPref = null;
        if(IDGameObjectPool.TryGetValue(prefabId,out objPref))
        {
            var result =  GameObject.Instantiate(objPref, position, rotation);
            result.SetActive(false);
            return result;
        }
        else
        {
            Debug.LogError("Prefab " + prefabId + " not founded in object pool");
            return null;
        }
    }
    
}
