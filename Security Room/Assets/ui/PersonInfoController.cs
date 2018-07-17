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

	public Text ageUncertainty;
	public Text heightUncertainty;
	public Text weightUncertainty;

	public RadarChart personalityChart; 

    private IEnumerator updater;

	private string currentSubjectId  = "";
	void Start() {
        if (Dummy.ENABLED) {
        	setPersonInfo(Dummy.getDummyPersonInfo());
        } else {
			updater = updateBars();
			StartCoroutine(updater);
		}
	}

	IEnumerator updateBars() {
		while (true) {
			WWW www = new WWW(Utility.SUBJECT_URL);
			yield return www;

			//if (www.error == null) {
			string jsonString = www.text;
			setPersonInfo(PersonInfo.createFromJsonString(jsonString));

			//} else {
			//	print("general info url not reachable: " + Utility.SUBJECT_URL);
			//}

			yield return new WaitForSeconds(Utility.UPDATE_INTERVAL);
		}
	}

	private void setPersonInfo(PersonInfo personInfo) {
		SharedInfo.subjectId = personInfo.id;
		if (!currentSubjectId.Equals(SharedInfo.subjectId)) {
			currentSubjectId = SharedInfo.subjectId;
			subjectId.text = currentSubjectId;
			if (personInfo.age > 0)
				age.text = personInfo.age + "";
			if (personInfo.gender != "")
				gender.text = personInfo.gender;
			if (personInfo.height < 3) {
				personInfo.height *= 100;
			}
			height.text = personInfo.height + "";
			weight.text = personInfo.weight + "";

			setUncertainty();
			for (int i = 0; i < personalityChart.GetParameters().Count; i++) {
				personalityChart.SetParameter(i, 0);
			}
		}
	}

	private void setUncertainty() {
		float guessAge = Random.Range(1, 4);
		float guessHeight = Random.Range(3, 12);
		float guessWeight = Random.Range(4, 10);

		ageUncertainty.text = "(± " + guessAge + ")";
		weightUncertainty.text = "(± " + guessHeight + ")";
		heightUncertainty.text = "(± " + guessHeight + ")";
	}




}