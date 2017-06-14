using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraImage : MonoBehaviour {
	public string url = "localhost:3000/image";
	private RawImage raw_img;
	private Texture2D tex;
	private float waiting_time = 0.5f;

	void Start () {
		raw_img = GetComponent<RawImage> ();
		tex = new Texture2D (1, 2, TextureFormat.DXT1, false);

		StartCoroutine (update_and_redraw ());
	}

	IEnumerator update_and_redraw() {
		while (true) {
				WWW www = new WWW (url);
				yield return www;
			if (www.error == null) {
				www.LoadImageIntoTexture (tex);
				raw_img.texture = tex;
			} else {
				print ("image url not reachable");
			}

			print ("waiting");
			yield return new WaitForSeconds (waiting_time);
			print ("waited");

		}
	}
}
