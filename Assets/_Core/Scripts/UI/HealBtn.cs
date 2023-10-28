using UnityEngine;
using UnityEngine.UI;

public class HealBtn : MonoBehaviour
{
    [SerializeField] private Image imgHeal;

    public void BtnPressed()
    {
        imgHeal.color = InputManager.Instance.PressColor;
    }

    public void BtnReleased()
    {
        imgHeal.color = InputManager.Instance.DefaultColor;
    }
}
