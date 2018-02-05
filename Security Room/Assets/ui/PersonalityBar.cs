using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalityBar : MonoBehaviour {
	private static readonly float MAX_BAR_SIZE = 50;

	public GameObject neuroBar;
	public GameObject openBar;
	public GameObject conscBar;
	public GameObject agreeBar;
	public GameObject extrovBar;

	// Use this for initialization
	void Start() {
		setScaleInPercent(neuroBar, 100);
		setScaleInPercent(openBar, 100);
		setScaleInPercent(conscBar, 100);
		setScaleInPercent(agreeBar, 50);
		setScaleInPercent(extrovBar, 30);

		// StartCoroutine(updateBars());
	}

	IEnumerator updateBars() {
		while (true) {
			WWW www = new WWW(Utility.PERSONALITY_URL);
			yield return www;
			if (www.error == null) {
				string jsonString = www.text;
				PersonalityInfo personalityInfo = PersonalityInfo.createFromJsonString(jsonString);
				setScaleInPercent(neuroBar, personalityInfo.personality1);
				setScaleInPercent(openBar, personalityInfo.personality2);
				setScaleInPercent(conscBar, personalityInfo.personality3);
				setScaleInPercent(agreeBar, personalityInfo.personality4);
				setScaleInPercent(extrovBar, personalityInfo.personality5);
			} else {
				print("personality url not reachable: " + Utility.PERSONALITY_URL);
			}

			print("waiting");
			yield return new WaitForSeconds(Utility.UPDATE_INTERVAL);
			print("waited");

		}
	}

	/**
	 * percentValue: 0-100?
	 **/
	void setScaleInPercent(GameObject bar, float percentValue) {
		setScale(bar, percentValue / 100 * MAX_BAR_SIZE);
	}

	void setScale(GameObject bar, float value) {
		Vector3 oldLocalScale = bar.transform.localScale;
		Vector3 newLocalScale = new Vector3(value, oldLocalScale.y, oldLocalScale.z);
		Vector3 diffScale = newLocalScale - oldLocalScale;
		bar.transform.localScale = newLocalScale;
		bar.transform.localPosition += diffScale / 2;
	}
}