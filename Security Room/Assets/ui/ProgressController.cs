
using UnityEngine;

public class ProgressController : MonoBehaviour {
	public GameObject uploading;
	public GameObject synchronized;

	private bool isUploading = true;

	private float minWaitingTime = 5;
	private float maxWaitingTime = 15;
	private float timeToChange;

	void Start() {
		updateState(isUploading);

		timeToChange = Time.time + getRandomWaitingTime();
	}

	void Update() {
		switchState();
	}

	private float getRandomWaitingTime() {
		return Random.Range(minWaitingTime, maxWaitingTime);
	}

	private void switchState() {
		float currentTime = Time.time;
		if (currentTime > timeToChange) {
			timeToChange = Time.time + getRandomWaitingTime();
			isUploading = !isUploading;
			updateState(isUploading);
		}
	}
	
	private void updateState(bool isUploading) {
		uploading.SetActive(isUploading);
		synchronized.SetActive(!isUploading);
	}
}