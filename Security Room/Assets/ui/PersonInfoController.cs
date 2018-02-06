using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonInfoController : MonoBehaviour {
	public Text subjectId;
	public Text age;
	public Text gender;
	public Text height;
	public Text weight;
	// Use this for initialization
	void Start() {
		if (Dummy.ENABLED) {
			setPersonInfo(Dummy.getDummyPersonInfo());
		} else {
			StartCoroutine(updateBars());

		}
	}

	IEnumerator updateBars() {
		while (true) {
			WWW www = new WWW(Utility.GENERAL_INFO_URL);
			yield return www;
			if (www.error == null) {
				string jsonString = www.text;
				setPersonInfo(PersonInfo.createFromJsonString(jsonString));
			} else {
				print("general info url not reachable: " + Utility.GENERAL_INFO_URL);
			}

			yield return new WaitForSeconds(Utility.UPDATE_INTERVAL);
		}
	}

	private void setPersonInfo(PersonInfo personInfo) {
		subjectId.text = personInfo.id;
		age.text = personInfo.age;
		gender.text = personInfo.gender;
		height.text = personInfo.height;
		weight.text = personInfo.weight;
	}

}