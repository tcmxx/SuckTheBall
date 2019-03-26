using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AblityDeck
{
    public List<string> abilities;

    private List<string> remaining;
    
    public AblityDeck()
    {
        abilities = new List<string>();
        remaining = new List<string>();
    }


    public void LoadDefaultAbilities()
    {
        abilities.Add("Beam");
        abilities.Add("BeamStatic");
        abilities.Add("Beam");
        abilities.Add("BeamStatic");
        abilities.Add("Beam");
        abilities.Add("BeamStatic");
        abilities.Add("Beam");
        abilities.Add("BeamStatic");
        abilities.Add("Beam");
        abilities.Add("BeamStatic");
    }

    public virtual string Next()
    {
        if(remaining == null || remaining.Count == 0)
        {
            remaining = new List<string>();
            remaining.AddRange(abilities);
            Shuffle(remaining);
        }

        var result = remaining[remaining.Count - 1];
        remaining.RemoveAt(remaining.Count - 1);
        return result;
    }

    protected void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0,n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
