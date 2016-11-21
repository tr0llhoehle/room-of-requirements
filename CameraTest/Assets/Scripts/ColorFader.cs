// fade background color between 2 colors
using UnityEngine;
using System.Collections;

public class ColorFader : MonoBehaviour {
	public Color color1 = Color.red;
	public Color color2 = Color.blue;
	public float duration = 5.0F;
	public Camera cam;

	void Start() {
		cam = GetComponent<Camera>();
		cam.clearFlags = CameraClearFlags.SolidColor;
	}

	void Update() {
		// fade between 2 colors
		float t = Mathf.PingPong(Time.time, duration) / duration;
		cam.backgroundColor = Color.Lerp(color1, color2, t);
	}
}