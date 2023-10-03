using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Position all gui into the current screen safearea
/// </summary>
public class UISafeArea : MonoBehaviour
{
    private RectTransform safeAreaTransform = null;

    Rect safeAreaRect;
    Vector2 minAnchor;
    Vector2 maxAnchor;

    private void Awake()
    {
        safeAreaTransform = GetComponent<RectTransform>();
        safeAreaRect = Screen.safeArea;
        minAnchor = safeAreaRect.position;
        maxAnchor = minAnchor + safeAreaRect.size;

        minAnchor.x /= Screen.width;
        minAnchor.y /= Screen.height;
        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;

        safeAreaTransform.anchorMin = minAnchor;
        safeAreaTransform.anchorMax = maxAnchor;
    }
}
