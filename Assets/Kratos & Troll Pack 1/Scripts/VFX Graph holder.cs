using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXGraphholder : MonoBehaviour
{
    [SerializeField] private List<VisualEffect> effects;

    public void Play(Troll_Manager.Attacks attack)
    {
        switch (attack)
        {
            case Troll_Manager.Attacks.StoneSmash:
                effects[0].Play();
                effects[1].Play();
                break;
            case Troll_Manager.Attacks.RightLegStomp:
                effects[2].Play();
                break;
            case Troll_Manager.Attacks.LeftLegStomp:
                effects[3].Play();
                break;
            case Troll_Manager.Attacks.FirstSweep:
                effects[4].Play();
                break;
            case Troll_Manager.Attacks.SecondSweep:
                effects[5].Play();
                break;
        }
    }

    public void Stop()
    {
        foreach (var effect in effects)
        {
            effect.Stop();
        }
    }
}
