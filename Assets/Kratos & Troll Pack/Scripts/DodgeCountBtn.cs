using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Calculate dodge button pressed count.
/// </summary>
public class DodgeCountBtn : MonoBehaviour
{
    public int PressCount { get; private set; }

    private void Start()
    {
        PressCount = 0;
    }

    // Public Methods
    public void DodgeBtnDown()
    {
        PressCount = PressCount >= 2 ? 0 : PressCount + 1;
    }

    public void ResetPressCount()
    {
        PressCount = 0;
    }
}
