using UnityEngine;
using System.Collections;

public class ColorChanger : MonoBehaviour {

	Renderer rd;
	float h, s, v;
	float delta = 0.01f;

	// Use this for initialization
	void Start () {
		rd = GetComponent<Renderer> ();
		Color.RGBToHSV(rd.material.color, out h, out s, out v);
	}
	
	// Update is called once per frame
	void Update () {
		// "rotate" hue component of the color
		rd.material.color = Color.HSVToRGB (h += delta, s, v);
		if (h > 1.0f) {
			h = 0;
		}
	}
}
