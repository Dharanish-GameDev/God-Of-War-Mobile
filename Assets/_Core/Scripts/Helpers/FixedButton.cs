using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handle the Button functions such as press, hold, release
/// </summary>
public class FixedButton : MonoBehaviour
{
    // Private Variables
    private int count;
    private bool isHold;

    // Properties
    public bool IsPressed { get; private set; }

    private void Start()
    {
        count = 0;
        isHold = false;
    }

    private void Update()
    {
        // wait for one frame and reset ispressed
        if (!IsPressed || isHold) return;

        count++;
        IsPressed = count <= 1;
    }

    // Public Methods
    public void BtnPressDown()
    {
        isHold = false;
        IsPressed = true;
    }

    public void BtnPress()
    {
        isHold = true;
        IsPressed = true;
    }

    public void BtnPressUp()
    {
        count = 0;
        isHold = false;
        IsPressed = false;
    }
}
