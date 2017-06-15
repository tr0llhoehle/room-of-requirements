using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalityBar : MonoBehaviour {
	private static readonly float MAX_BAR_SIZE = 50;

	private GameObject bar1;
	private GameObject bar2;
	private GameObject bar3;
	private GameObject bar4;
	private GameObject bar5;

	// Use this for initialization
	void Start () {
		bar1 = GameObject.Find ("bar1");
		bar2 = GameObject.Find ("bar2");
		bar3 = GameObject.Find ("bar3");
		bar4 = GameObject.Find ("bar4");
		bar5 = GameObject.Find ("bar5");

		StartCoroutine (updateBars ());
	}
	
	IEnumerator updateBars() {
		while (true) {
			WWW www = new WWW (Utility.PERSONALITY_URL);
			yield return www;
			if (www.error == null) {
				string jsonString = www.text;
				PersonalityInfo personalityInfo = PersonalityInfo.createFromJsonString (jsonString);
				setScaleInPercent (bar1, personalityInfo.personality1);
				setScaleInPercent (bar2, personalityInfo.personality2);
				setScaleInPercent (bar3, personalityInfo.personality3);
				setScaleInPercent (bar4, personalityInfo.personality4);
				setScaleInPercent (bar5, personalityInfo.personality5);
			} else {
				print ("personality url not reachable: " + Utility.PERSONALITY_URL);
			}

			print ("waiting");
			yield return new WaitForSeconds (Utility.UPDATE_INTERVAL);
			print ("waited");

		}
	}

	/**
	 * percentValue: 0-100?
	 **/
	void setScaleInPercent(GameObject bar, float percentValue) {
		setScale (bar, percentValue / 100 * MAX_BAR_SIZE);
	}

	void setScale(GameObject bar, float value) {
		Vector3 oldLocalScale = bar.transform.localScale;
		Vector3 newLocalScale = new Vector3 (value, oldLocalScale.y, oldLocalScale.z);
		Vector3 diffScale = newLocalScale - oldLocalScale;
		bar.transform.localScale = newLocalScale;
		bar.transform.localPosition += diffScale / 2;
	}
}
