using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamagableObject : MonoBehaviour, IDamagable
{
    public float initialHP = 10;

    public float currentHP;

    [SerializeField]
    protected bool HPLinkLifetime = false;//if true, hp will be synced with lifetime;

    public UnityEvent onDestroy;

    [SerializeField]
    protected AbilityObject abilityObjectRef;

    private void Awake()
    {
        Debug.Assert(abilityObjectRef != null || HPLinkLifetime == false, name + "HP is linked with object life time but no AbilityObject script is attached to the game object");
        currentHP = initialHP;
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        if (!HPLinkLifetime && currentHP <=0)
        {
            onDestroy.Invoke();
            if(abilityObjectRef)
                abilityObjectRef.NetworkDestroy();
            
        }
    }
}
