using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles switch btn light and heavy attacks in both hand and axe.
/// </summary>
public class SwitchAttackBtn : MonoBehaviour
{
    [SerializeField] private Image imgSwitchAttack;
    [SerializeField] private Sprite lSprite;
    [SerializeField] private Sprite hSprite;

    // Properties
    public bool IsLAttack { get; private set; }

    private void Start()
    {
        IsLAttack = true;
    }

    // Public methods
    public void SwitchAttack()
    {
        // change attack and update ui
        IsLAttack = !IsLAttack;
        imgSwitchAttack.sprite = IsLAttack ? lSprite : hSprite;
    }

    public void BtnPressed()
    {
        imgSwitchAttack.color = InputManager.Instance.PressColor; 
    }

    public void BtnReleased()
    {
        imgSwitchAttack.color = InputManager.Instance.DefaultColor;
    }
}
