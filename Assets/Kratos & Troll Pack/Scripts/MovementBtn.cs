using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MovementBtn : MonoBehaviour
{
    [SerializeField] private Image runImage;

    // Private Variables
    private bool isBtnReleased;

    // Properties
    public bool IsRunPressed { get; private set; }

    private void Start()
    {
        IsRunPressed = false;
        runImage.gameObject.SetActive(false);
    }

    // Public Methods
    public void BtnPointerDown()
    {
        isBtnReleased = false;
        IsRunPressed = false;
        runImage.color = InputManager.Instance.DefaultColor;
        runImage.gameObject.SetActive(true);
    }

    public void BtnPointerUp()
    {
        isBtnReleased = true;

        if (IsRunPressed) return;
        runImage.gameObject.SetActive(false);
        InputManager.Instance.MovementJoystick.PointerUp();
    }

    public void RunBtnEnter()
    {
        if (isBtnReleased) return;
        IsRunPressed = true;
        runImage.color = InputManager.Instance.PressColor;
    }

    public void RunBtnExit()
    {
        if (isBtnReleased) return;
        IsRunPressed = false;
        runImage.color = InputManager.Instance.DefaultColor;
    }

    public void StopRun()
    {
        if (!IsRunPressed) return;
        IsRunPressed = false;
        BtnPointerUp();
    }
}
