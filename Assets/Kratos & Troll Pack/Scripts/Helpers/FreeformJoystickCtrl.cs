using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Controls The Freeform Joystick Behaviour and Give The Input Based On Joystick Direction 
/// </summary>
public class FreeformJoystickCtrl : MonoBehaviour
{
    public class DropEventArgs : EventArgs
    {
        public Vector2 direction;
    } 

    // Events
    public event EventHandler OnDrag;
    public event EventHandler<DropEventArgs> OnDrop;
    private readonly DropEventArgs dropEventArgs = new();

    private enum JoystickMode { FixedStatic, FixedDynamic, Dynamic }
    private enum InputMode { Smooth, Raw }

    [SerializeField] private JoystickMode mode = JoystickMode.FixedStatic;
    [SerializeField] private InputMode input = InputMode.Smooth;

    [Header("UI")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasScaler canvasScaler;

    [Header("Settings")]
    [SerializeField] private RectTransform bg;
    [SerializeField] private RectTransform stick;
    [SerializeField] private float distance = 100.0f;
    [SerializeField] [Range(0.0f, 1.0f)] private float threshold = 0.1f;
    [SerializeField] private float smoothAmount = 2f;

    // Private Variables
    [HideInInspector] private Vector2 dir, inputDir;  
    [HideInInspector] private float dist;
    [HideInInspector] private Vector2 touchPos;
    [HideInInspector] private Vector2 originPos;
    [HideInInspector] private PointerEventData _eventData;
    [HideInInspector] private bool isPressed;

    // Properties
    public float Horizontal 
    { 
        get 
        {
            if (input == InputMode.Raw) return Mathf.RoundToInt(Direction.x);
            return inputDir.x; 
        } 
    }                                     // get the horizontal input from 0 to 1
    public float Vertical
    {
        get
        {
            if (input == InputMode.Raw) return Mathf.RoundToInt(Direction.y);
            return inputDir.y;
        }
    }                                        // get the vertical input from 0 to 1
    public Vector2 Direction { get; private set; }                  // get normalized direction of the joystick
    public float MaxDistance { get { return distance; } }           // get the maximum distance that stick can able to move
    public float Distance { get; private set; }                     // get the current distance of the stick from the bg
    public float ThresholdDistance { get; private set; }            // get the threshold distance for the joystick
    public bool IsMoving { get; private set; }                      // get the stick is moving or not

    private void Start()
    {
        isPressed = false;

        // origin position of the joystick
        originPos = transform.position;             

        // updates the analog distance based on the device screen resolution
        UpdateAnalogDistance();
    }

    private void Update()
    {
        CalculateStickIsMoving();

        // update smooth input dir
        if (input == InputMode.Raw || !isPressed) return;
        inputDir = Vector2.MoveTowards(inputDir, dir, Time.deltaTime * smoothAmount);
    }

    // UI Event Methods
    public void PointerDown(BaseEventData baseEventData)
    {
        // don't call event when gameobject not active
        if (!gameObject.activeSelf) return;

        isPressed = true;
        _eventData = (PointerEventData)baseEventData; 
        touchPos = _eventData.position;

        // change joystick to touchpos
        if (mode != JoystickMode.FixedStatic)
        {
            bg.position = _eventData.position;
            stick.position = _eventData.position;
        }

        // call event
        OnDrag?.Invoke(this, EventArgs.Empty);
    }

    public void Dragging(BaseEventData baseEventData)
    {
        // don't call event when gameobject not active
        if (!gameObject.activeSelf) return;

        // get event data from the event system
        _eventData = (PointerEventData)baseEventData;

        switch (mode)                   // move joystick based on the modes
        {
            case JoystickMode.FixedStatic: FixedStaticMode(); break;
            case JoystickMode.FixedDynamic: FixedDynamicMode(); break;
            case JoystickMode.Dynamic: DynamicMode(); break;
        }
    }

    public void PointerUp()
    {
        // don't call event when gameobject not active
        if (!gameObject.activeSelf) return;

        isPressed = false;
        // fire event
        if (Distance >= ThresholdDistance)
        {
            dropEventArgs.direction = dir;
            OnDrop?.Invoke(this, dropEventArgs);
        }

        // reset joystick
        bg.position = originPos;
        stick.position = originPos;

        // reset values
        Distance = 0.0f;
        touchPos.x = 0.0f;
        touchPos.y = 0.0f;
        dir.x = 0.0f;
        dir.y = 0.0f;
        dist = 0.0f;

        // reset proerties
        Direction = dir;
        inputDir = dir;
    }

    // Private Methods
    private void FixedStaticMode()
    {
        // calculate direction
        dir = _eventData.position - originPos;
        Distance = Mathf.Clamp(dir.magnitude, 0.0f, distance);
        dir.Normalize();

        // update direction when we exceed the threshold
        Direction = (Distance >= ThresholdDistance) ? dir : Vector2.zero;

        // move the stick
        dist = Vector2.Distance(originPos, _eventData.position);
        if (dist < distance) stick.position = originPos + dir * dist;
        else stick.position = originPos + dir * distance;
    }

    private void FixedDynamicMode()
    {
        // calculate direction
        dir = _eventData.position - touchPos;
        Distance = Mathf.Clamp(dir.magnitude, 0.0f, distance);
        dir.Normalize();

        // update direction when we exceed the threshold
        Direction = (Distance >= ThresholdDistance) ? dir : Vector2.zero;

        // move the stick
        dist = Vector2.Distance(touchPos, _eventData.position);
        if (dist < distance) stick.position = touchPos + dir * dist;
        else stick.position = touchPos + dir * distance;
    }

    private void DynamicMode()
    {
        // calculate direction
        dir = _eventData.position - touchPos;
        Distance = Mathf.Clamp(dir.magnitude, 0.0f, distance);
        dir.Normalize();

        // update direction when we exceed the threshold
        Direction = (Distance >= ThresholdDistance) ? dir : Vector2.zero;

        // move the stick
        dist = Vector2.Distance(touchPos, _eventData.position);
        if (dist <= distance) stick.position = _eventData.position;
        else        // move the bg along with stick
        {
            stick.position = _eventData.position;
            bg.position = (Vector2)stick.position - (dir * distance);
            touchPos = bg.position;
        }
    }

    private void UpdateAnalogDistance()
    {
        Vector2 refRes, scrRes, difRes;
        float multiplier;

        // initialize variable
        refRes = canvasScaler.referenceResolution;          // referece resolution in the canvas scalar
        scrRes = canvas.renderingDisplaySize;               // current screen resolution
        difRes = refRes - scrRes;                           // difference between current and reference resolution

        // calculate distance multiplier
        if (scrRes.magnitude <= refRes.magnitude) 
            multiplier = Mathf.Lerp(0, 1, Mathf.InverseLerp(refRes.magnitude, 0, difRes.magnitude));
        else
            multiplier = Mathf.Lerp(1.01f, 2, Mathf.InverseLerp(0, refRes.magnitude, difRes.magnitude));

        // update the distance
        distance *= multiplier;
        ThresholdDistance = Mathf.Lerp(0.0f, distance, threshold);
    }

    private void CalculateStickIsMoving()
    {
        if (!stick.hasChanged)
        {
            IsMoving = false;
            return;
        }

        // stick moving
        IsMoving = true;
        stick.hasChanged = false;
    }
}
