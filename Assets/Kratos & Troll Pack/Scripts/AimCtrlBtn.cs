using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Handles the axe aim button behaviour such as aim, throw.
/// </summary>
public class AimCtrlBtn : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private FixedButton aimBtn;
    [SerializeField] private AxePickBtn aimPickBtn;
    [SerializeField] private GameObject throwBG;
    [SerializeField] private GameObject cancelBtn;
    [SerializeField] private Image imgThrow;
    [SerializeField] private Image imgAxePickup;

    // Private Variables
    private int count;
    private bool isCancelAim, isRecallBtn;

    // Properties
    public bool IsAxeThrow { get; private set; }
    public bool IsAxeRecallBtn { get; private set; }

    private void Start()
    {
        count = 0;
        isCancelAim = false;
        IsAxeThrow = false;

        throwBG.SetActive(false);
        cancelBtn.SetActive(false);
        DisableAimCtrlBtn();
    }

    private void Update()
    {
        // enable aim joystick after axe returned
        if (isRecallBtn)
        {
            if (aimBtn.IsPressed && !IsAxeRecallBtn)
            {
                isRecallBtn = false;
                throwBG.SetActive(true);
                cancelBtn.SetActive(true);
            }
        }

        // reset isaxethrow after a next frame
        if (!IsAxeThrow) return;
        count += 1;
        IsAxeThrow = count <= 1;
    }

    // Public Methods
    public void AimJoystickPointerDown(BaseEventData baseEventData)
    {
        if (IsAxeRecallBtn) return;
        InputManager.Instance.AimJoystick.PointerDown(baseEventData);
    }

    public void AimJoystickPointerDrag(BaseEventData baseEventData)
    {
        if (IsAxeRecallBtn) return;
        InputManager.Instance.AimJoystick.Dragging(baseEventData);
    }

    public void AimJoystickPointerUp()
    {
        if (IsAxeRecallBtn) return;
        InputManager.Instance.AimJoystick?.PointerUp();
    }

    public void EnableAimCtrlBtn()
    {
        imgThrow.color = InputManager.Instance.DefaultColor;
        imgThrow.raycastTarget = true;
    }

    public void DisableAimCtrlBtn()
    {
        imgThrow.color = InputManager.Instance.DisableColor;
        imgThrow.raycastTarget = false;
    }

    public void ChangeAttackBtnToHand()
    {
        // change attack button sprite to hand
        aimPickBtn.SetHandSprite();
    }

    public void ChangeAttackBtnToAxe()
    {
        // change attack button sprite to axe
        aimPickBtn.SetAxeSprite();
    }

    public void ChangeAimJoystickToRecallBtn()
    {
        IsAxeRecallBtn = true;
        isRecallBtn = true;

        // disable axepickup btn
        imgAxePickup.raycastTarget = false;
        imgAxePickup.color = InputManager.Instance.DisableColor;

        ChangeAttackBtnToHand();
    }

    public void ChangeRecallBtnAimJoystick()
    {
        IsAxeRecallBtn = false;

        // enable axepickup btn
        imgAxePickup.raycastTarget = true;
        imgAxePickup.color = InputManager.Instance.DefaultColor;

        ChangeAttackBtnToAxe();
    }

    public void AimJoystickDown()
    {
        count = 0;
        IsAxeThrow = false;
        isCancelAim = false;
        imgThrow.color = InputManager.Instance.PressColor;

        if (IsAxeRecallBtn) return;
        throwBG.SetActive(true);
        cancelBtn.SetActive(true);
    } 

    public void AimJoystickUp()
    {
        IsAxeThrow =!isCancelAim;
        imgThrow.color = Color.white;

        if (IsAxeRecallBtn) return;
        throwBG.SetActive(false);
        cancelBtn.SetActive(false);
    }

    public void CancelAimEnter()
    {
        isCancelAim = true;
    }

    public void CancelAimExit()
    {
        isCancelAim = false;
    }
}
