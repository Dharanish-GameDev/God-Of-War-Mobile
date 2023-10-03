using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HAttackPoint : MonoBehaviour
{
    public bool CanPlace { get; set; }

    private void OnTriggerStay(Collider other)
    {
        CanPlace = !other.gameObject.layer.Equals(7);
    }
}
