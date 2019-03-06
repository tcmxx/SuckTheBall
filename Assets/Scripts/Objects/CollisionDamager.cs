using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDamager : MonoBehaviour
{
    public LayerMask effectedLayer;

    public AnimationCurve enterImpulseDamageCurve = AnimationCurve.Constant(0,1,1);
    public float maxEnterImpulse = 5;
    public float minEnterImpulse = 0;
    public float maxEnterDamage = 2;

    public AnimationCurve stayImpulseDamageCurve = AnimationCurve.Constant(0, 1, 1);
    public float maxStayImpulse = 5;
    public float minStayImpulse = 0;
    public float maxStayDamage = 0;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & effectedLayer) != 0)
        {
            var damagable = collision.gameObject.GetComponent<IDamagable>();
            if (damagable != null)
            {
                float impulse = collision.contacts[0].normalImpulse;
                Debug.Log(name + " collision enter with impulse:" + impulse);
                float damage = maxEnterDamage * enterImpulseDamageCurve.Evaluate(Mathf.Clamp01((impulse - minEnterImpulse) / (maxEnterImpulse - minEnterImpulse)));
                damagable.TakeDamage(damage);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & effectedLayer) != 0)
        {
            var damagable = collision.gameObject.GetComponent<IDamagable>();
            if (damagable != null)
            {
                float impulse = collision.contacts[0].normalImpulse;
                Debug.Log(name + " collision stay with impulse:" + impulse);
                float damage = maxStayDamage * stayImpulseDamageCurve.Evaluate(Mathf.Clamp01((impulse - minStayImpulse) / (maxStayImpulse - minStayImpulse)));
                damagable.TakeDamage(damage);
            }
        }
    }
}
