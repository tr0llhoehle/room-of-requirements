using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraImage : MonoBehaviour {
	public RawImage img1;
	public RawImage img2;
	public RawImage img3;
	public RawImage img4;
	public RawImage img5;

	private Texture2D tex1;
	private Texture2D tex2;
	private Texture2D tex3;
	private Texture2D tex4;
	private Texture2D tex5;

	private int currentActiveImage = 0;

	public float startTime;

	private IEnumerator updateRedraw;

	void Start() {
		//if (Dummy.ENABLED) {

		//} else {
		tex1 = new Texture2D(1, 2, TextureFormat.DXT1, false);
		tex2 = new Texture2D(1, 2, TextureFormat.DXT1, false);
		tex3 = new Texture2D(1, 2, TextureFormat.DXT1, false);
		tex4 = new Texture2D(1, 2, TextureFormat.DXT1, false);
		tex5 = new Texture2D(1, 2, TextureFormat.DXT1, false);

		startTime = Time.time;

		updateRedraw = updateAndRedraw();

		StartCoroutine(updateRedraw);

		//}
	}

	private void Update() {
		/*
		if (Time.time - startTime <= 2.0f) {
			return;
		}
		currentActiveImage++;
		if (currentActiveImage == 5) {
			currentActiveImage = 0;
		}
		startTime = Time.time;
		rotateActiveImages();
        */
	}

	IEnumerator updateAndRedraw() {
		while (true) {
			WWW www = new WWW(Utility.IMAGE_URL);
			yield return www;

			//if (www.error == null) {

			Texture2D tex = getCurrentTexture();
			www.LoadImageIntoTexture(tex);

			currentActiveImage++;
			if (currentActiveImage == 5) {
				currentActiveImage = 0;
			}
			getCurrentImage().texture = tex;
			getCurrentImage().transform.SetAsLastSibling();

			//} else {
			//	print("image url not reachable: " + Utility.IMAGE_URL);
			//}

			yield return new WaitForSeconds(Utility.UPDATE_INTERVAL * 3);

		}
	}

	private RawImage getCurrentImage() {
		switch (currentActiveImage) {
			case 0:
				return img1;
			case 1:
				return img2;
			case 2:
				return img3;
			case 3:
				return img4;
			case 4:
			default:
				return img5;
		}
	}

	private Texture2D getCurrentTexture() {
		switch (currentActiveImage) {
			case 0:
				return tex1;
			case 1:
				return tex2;
			case 2:
				return tex3;
			case 3:
				return tex4;
			case 4:
			default:
				return tex5;
		}
	}
}