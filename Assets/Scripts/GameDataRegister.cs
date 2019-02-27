using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class GameDataRegister : ScriptableObject
{
    [SerializeField]
    protected List<AbilityInfo> abilityInfoRegister;
    public Dictionary<string, AbilityInfo> AbilityRegister { get {
            if(abilityRegister == null)
            {
                abilityRegister = new Dictionary<string, AbilityInfo>();
                foreach (var a in abilityInfoRegister)
                {
                    abilityRegister[a.abilityID] = a;
                }
                return abilityRegister;
            }
            else
            {
                return abilityRegister;
            }

        } }

    protected Dictionary<string, AbilityInfo> abilityRegister = null;


}
