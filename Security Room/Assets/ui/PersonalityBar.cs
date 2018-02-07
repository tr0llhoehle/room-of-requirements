using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalityBar : MonoBehaviour {
	private static readonly float MAX_BAR_SIZE = 250;

	public GameObject neuroBar;
	public GameObject openBar;
	public GameObject conscBar;
	public GameObject agreeBar;
	public GameObject extrovBar;

	private string currentSubjectId = "";
	// Use this for initialization
	void Start() {
		//if (Dummy.ENABLED) {
		//	setAllBars(Dummy.getDummyRandomizedGeneralizedPersonality());
		//} else {
			StartCoroutine(updateBars());
		//}

	}

	IEnumerator updateBars() {
		while (true) {
            WWW www = new WWW(Utility.SUBJECT_URL);
			yield return www;
			if (www.error == null) {
				string jsonString = www.text;
				setAllBars(GeneralizedPersonality.createFromJsonString(jsonString));
			} else {
				print("personality url not reachable: " + Utility.SUBJECT_URL);
			}

			yield return new WaitForSeconds(Utility.UPDATE_INTERVAL);
		}
	}

	private void setAllBars(GeneralizedPersonality personalityInfo) {
		if (!currentSubjectId.Equals(SharedInfo.subjectId)) {
			currentSubjectId = SharedInfo.subjectId;
		}
		setScaleInPercent(neuroBar, personalityInfo.neuroticism);
		setScaleInPercent(openBar, personalityInfo.openness);
		setScaleInPercent(conscBar, personalityInfo.conscientiousness);
		setScaleInPercent(agreeBar, personalityInfo.agreeableness);
		setScaleInPercent(extrovBar, personalityInfo.extroversion);
	}

	/**
	 * percentValue: 0-100?
	 **/
	private void setScaleInPercent(GameObject bar, float percentValue) {
		setScale(bar, percentValue / 100 * MAX_BAR_SIZE);
	}

	private void setScale(GameObject bar, float value) {
		Vector3 oldLocalScale = bar.transform.localScale;
		Vector3 newLocalScale = new Vector3(value, oldLocalScale.y, oldLocalScale.z);
		Vector3 diffScale = newLocalScale - oldLocalScale;
		bar.transform.localScale = newLocalScale;
		bar.transform.localPosition += diffScale / 2;
	}
}