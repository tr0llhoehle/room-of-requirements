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

	private int currentActiveImage = 0;
	private Texture2D tex;
	private Texture2D previousTex;

	public float startTime;

	void Start() {
		if (Dummy.ENABLED) {

		} else {
			tex = new Texture2D(1, 2, TextureFormat.DXT1, false);
			startTime = Time.time;
			StartCoroutine(updateAndRedraw());
		}
	}

	private void Update() {
		if (Time.time - startTime <= 2.0f) {
			return;
		}
		currentActiveImage++;
		if (currentActiveImage == 5) {
			currentActiveImage = 0;
		}
		startTime = Time.time;
		rotateActiveImages();
	}

	IEnumerator updateAndRedraw() {
		while (true) {
			if (Time.time - startTime <= 2.0f) {
				continue;
			}
			startTime = Time.time;
			WWW www = new WWW(Utility.IMAGE_URL);
			yield return www;
			if (www.error == null) {
				www.LoadImageIntoTexture(tex);
				currentActiveImage++;
				if (currentActiveImage == 5) {
					currentActiveImage = 0;
				}
				if (previousTex != null && previousTex.Equals(tex)) {
					rotateActiveImages();
				} else {
					updateImages(tex);
				}
				previousTex = tex;
			} else {
				print("image url not reachable: " + Utility.IMAGE_URL);
			}

			print("waiting");
			yield return new WaitForSeconds(Utility.UPDATE_INTERVAL);
			print("waited");

		}
	}

	private void rotateActiveImages() {
		RawImage img = null;
		switch (currentActiveImage) {
			case 0:
				img = img1;
				break;
			case 1:
				img = img2;
				break;
			case 2:
				img = img3;
				break;
			case 3:
				img = img4;
				break;
			case 4:
				img = img5;
				break;
			default:
				break;
		}
		img.transform.SetAsLastSibling();

	}

	private void updateImages(Texture2D tex) {
		switch (currentActiveImage) {
			case 0:
				img1.texture = tex;
				break;
			case 1:
				img2.texture = tex;
				break;
			case 2:
				img3.texture = tex;
				break;
			case 3:
				img4.texture = tex;
				break;
			case 4:
				img5.texture = tex;
				break;
			default:
				break;
		}
		rotateActiveImages();
	}
}