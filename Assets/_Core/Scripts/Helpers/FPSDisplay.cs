using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
	[SerializeField] private Color textColor = Color.black;
	[SerializeField] private int fontSize = 12;
	[SerializeField] private float updatePeriod = 0.5f;
	[SerializeField] private int lowFpsThreshold = 60;

	// Private Variables
	private float fps, msec;
	private float fpsAvg = 0f;
	private float msecAvg = 0f;
	private float lastUpdated = 0f;

	void Update()
	{
		if (Time.time - lastUpdated > updatePeriod)
		{
			fps = 1.0f / Time.unscaledDeltaTime;
			fpsAvg = (fpsAvg + fps) / 2;

			msec = Time.unscaledDeltaTime * 1000.0f;
			msecAvg = (msecAvg + msec) / 2;
			lastUpdated = Time.time;
		}
	}

	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(0, 0, w, h);

		style.alignment = TextAnchor.UpperCenter;
		style.fontSize = fontSize;
		style.normal.textColor = fpsAvg > lowFpsThreshold ? textColor : Color.red;

		string text = $"{fpsAvg:0.} FPS ({msecAvg:0.0}ms)";

		GUI.Label(rect, text, style);
	}
}