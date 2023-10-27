using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Give touch input direction when the player presses and releases the screen.
/// </summary>
public class InputTouchArea : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // Private Methods
    [HideInInspector] private Vector2 tempVec;
    [HideInInspector] private Vector2 oldPos;
    [HideInInspector] private int PointerId;
    [HideInInspector] private bool isPressed;

    // Properties
    public Vector2 TouchDist { get; private set; }

    private void Start()
    {
        isPressed = false;
    }

    void Update()
    {
        // check is not pressed the screen
        if (!isPressed) return;

        // get touch position from mobile
        if (PointerId >= 0 && PointerId < Input.touches.Length) 
            UpdateTouchDist(Input.touches[PointerId].position);
        // use mouse position as a touch position in pc
        else 
            UpdateTouchDist(Input.mousePosition);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // pressed the screen
        isPressed = true;
        PointerId = eventData.pointerId;
        oldPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // release the screen
        isPressed = false;
        ResetTouchDist();
    }

    // Private Methods
    private void ResetTouchDist()
    {
        // reset touch dist
        tempVec.x = 0;
        tempVec.y = 0;
        TouchDist = tempVec;
    }

    private void UpdateTouchDist(Vector2 currentTouchPos)
    {
        tempVec = currentTouchPos - oldPos;
        tempVec.Normalize();
        TouchDist = tempVec;
        oldPos = currentTouchPos;
    }
}
