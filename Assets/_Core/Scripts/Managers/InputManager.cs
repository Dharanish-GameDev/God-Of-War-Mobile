using System.Collections;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public enum InputType { Pc, Mobile }
    [SerializeField] private InputType type = InputType.Pc;

    //[SerializeField] private float touchSensitivity = 2;

    [Header("Btn Color")]
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color pressColor;
    [SerializeField] private Color disableColor;

    [Header("Joystick")]
    [SerializeField] private InputTouchArea touchArea;
    [SerializeField] private FreeformJoystickCtrl movementJoystick;
    [SerializeField] private FreeformJoystickCtrl aimJoystick;

    [Header("Button")]
    [SerializeField] private SwitchAttackBtn switchAttackBtn;
    [SerializeField] private MovementBtn movementBtn;
    [SerializeField] private FixedButton AxePickBtn;
    [SerializeField] private AimCtrlBtn aimCtrlBtn;
    [SerializeField] private FixedButton aimJoystickBtn;
    [SerializeField] private FixedButton attackBtn;
    [SerializeField] private FixedButton shieldBtn;
    [SerializeField] private FixedButton dodgeBtn;
    [SerializeField] private DodgeCountBtn dodgeCountBtn;
    [SerializeField] private FixedButton healBtn;

    // Private Variable
    private Vector2 mouseInput;

    // Properties
    public InputType Type { get { return type; } }
    public Color DefaultColor { get { return defaultColor; } }
    public Color PressColor { get { return pressColor; } }
    public Color DisableColor { get { return disableColor; } }

    public FreeformJoystickCtrl MovementJoystick { get { return movementJoystick; } }
    public FreeformJoystickCtrl AimJoystick { get { return aimJoystick; } }
    public MovementBtn MovementBtn { get { return movementBtn; } }
    public AimCtrlBtn AimCtrlBtn { get { return aimCtrlBtn; } }
    public DodgeCountBtn DodgeCountBtn { get { return dodgeCountBtn; } }
    public FixedButton HealBtn { get { return healBtn; } }

    public Vector2 TouchDist 
    { 
        get 
        { 
            if (type == InputType.Pc)
            {
                mouseInput.x = Input.GetAxis("Mouse X");
                mouseInput.y = -Input.GetAxis("Mouse Y");
                return mouseInput;
            }
            else
            {
                if (IsAimJoystickBtnPressed)
                {
                    mouseInput.x = aimJoystick.Horizontal * GameCtrl.Instance.AimCameraRotationSpeedValue.x;
                    mouseInput.y = -aimJoystick.Vertical * GameCtrl.Instance.AimCameraRotationSpeedValue.y;
                    return mouseInput;
                }
                else
                {
                    mouseInput.x = touchArea.TouchDist.x * GameCtrl.Instance.CameraRotationSpeedValue.x;
                    mouseInput.y = -touchArea.TouchDist.y * GameCtrl.Instance.CameraRotationSpeedValue.y;
                    return mouseInput;
                }
            }
        } 
    }
    public float Horizontal
    {
        get
        {
            return (type == InputType.Pc) ? Input.GetAxis("Horizontal") : movementJoystick.Horizontal;
        }
    }
    public float Vertical
    {
        get
        {
            return (type == InputType.Pc) ? Input.GetAxis("Vertical") : movementJoystick.Vertical;
        }
    }
    public bool IsRunPressed
    {
        get
        {
            return (type == InputType.Pc) ? Input.GetKey(KeyCode.LeftShift) : movementBtn.IsRunPressed;
        }
    }
    public bool IsLAttackButtonPressed
    {
        get
        {
            return (type == InputType.Pc) ? Input.GetMouseButtonDown(0) : switchAttackBtn.IsLAttack && attackBtn.IsPressed;
        }
    }
    public bool IsHAttackButtonPressed
    {
        get
        {
            return (type == InputType.Pc) ? Input.GetMouseButtonDown(1) : !switchAttackBtn.IsLAttack && attackBtn.IsPressed;
        }
    }
    public bool IsAxePick
    {
        get
        {
            return (type == InputType.Pc) ? Input.GetKeyDown(KeyCode.Alpha1) : AxePickBtn.IsPressed;
        }
    }
    public bool IsAimJoystickBtnPressed
    {
        get
        {
            return (type == InputType.Pc) ? Input.GetKey(KeyCode.LeftControl) : aimJoystickBtn.IsPressed;
        }
    }
    public bool IsAxeThrow
    {
        get
        {
            return (type == InputType.Pc) ? Input.GetMouseButtonDown(0) : aimCtrlBtn.IsAxeThrow;
        }
    }
    public bool IsAxeRecall
    {
        get
        {
            return (type == InputType.Pc) ? Input.GetKeyDown(KeyCode.R) : aimJoystickBtn.IsPressed && aimCtrlBtn.IsAxeRecallBtn;
        }
    }
    public bool IsShieldButtonPressed
    {
        get
        {
            return (type == InputType.Pc) ? Input.GetKey(KeyCode.Q) : shieldBtn.IsPressed;
            
        }
    }
    public bool IsDodgeBottonPressed
    {
        get
        {
            return (type == InputType.Pc) ? Input.GetKeyDown(KeyCode.Space) : dodgeBtn.IsPressed;
            
        }
    }
    public bool IsDodgeRollButtonPressed
    {
        get
        {
            return (type == InputType.Pc) ? Input.GetKeyDown(KeyCode.Space) : dodgeBtn.IsPressed && dodgeCountBtn.PressCount >= 2;

        }
    }
    public bool IsHealButtonPressed
    {
        get
        {
            return (type == InputType.Pc) ? Input.GetKeyDown(KeyCode.F) : healBtn.IsPressed;
        }
    }

    private void Awake()
    {
        Instance = this;

        healBtn.gameObject.SetActive(false);
    }
}
