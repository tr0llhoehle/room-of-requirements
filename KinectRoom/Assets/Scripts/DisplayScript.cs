using UnityEngine;
using System.Collections;

public class DisplayScript : MonoBehaviour
{
	// Use this for initialization
	void Start()
	{
		Debug.Log("displays connected: " + Display.displays.Length);
		// Display.displays[0] is the primary, default display and is always ON.
		// Check if additional displays are available and activate each. For the walls.
		if (Display.displays.Length == 4) {
			Debug.Log ("Using multi-display support");
			Display.displays [1].Activate();
			// This is the security room display
			Display.displays [2].Activate();
			//Display.displays [3].Activate();
        } else {
			Debug.Log ("Activating split screen mode.");
			Camera left = GameObject.Find ("Left Camera").GetComponent<Camera>();
			Camera middle = GameObject.Find ("Main Camera").GetComponent<Camera>();
			Camera right = GameObject.Find ("Right Camera").GetComponent<Camera>();
			left.rect = new Rect (0.0f, 0.0f, 1.0f/3.0f, 1.0f);
			middle.rect = new Rect (1.0f/3.0f, 0.0f, 1.0f/3.0f, 1.0f);
			right.rect = new Rect (2.0f/3.0f, 0.0f, 1.0f/3.0f, 1.0f);
			left.targetDisplay = 0;
			middle.targetDisplay = 0;
			right.targetDisplay = 0;
		}
	}
	// Update is called once per frame
	void Update()
	{

	}
}
