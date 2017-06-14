using UnityEngine;
using UnityEngine.UI;
using System;

//private Text text;
public class ClockAnimator : MonoBehaviour {
	

	private Text time_text;

	// Use this for initialization
	void Start () {
		time_text = gameObject.GetComponent<Text> ();
		time_text.text = "13:37";
	}

	// Update is called once per frame
	void Update () {
		DateTime time = DateTime.Now;
		time_text.text = time.ToString ("HH:mm");
	}

}