using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handle Axe Pickup and putdown.
/// </summary>
public class AxePickBtn : MonoBehaviour
{
    [SerializeField] private Image imgAxePickup;
    [SerializeField] private Image imgAttackBtn;
    [SerializeField] private Sprite handSprite;
    [SerializeField] private Sprite axeSprite;

    // Public Methods
    public void SetHandSprite()
    {
        imgAttackBtn.sprite = handSprite;
    }

    public void SetAxeSprite()
    {
        imgAttackBtn.sprite = axeSprite;
    }

    public void BtnPressed()
    {
        imgAxePickup.color = InputManager.Instance.PressColor;
    }

    public void BtnReleased()
    {
        imgAxePickup.color = InputManager.Instance.DefaultColor;
    }
}
