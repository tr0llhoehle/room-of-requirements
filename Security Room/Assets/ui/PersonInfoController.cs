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

	public TraitsChartController traitsChartController; 

    private IEnumerator updater;
	private int updateCount = 0;

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
		// if (!currentSubjectId.Equals(SharedInfo.subjectId)) {
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

		if (updateCount % 5 == 0) {
			setUncertainty();
		}
		if (updateCount > 15) {
			traitsChartController.resetPersonalityChart();
			updateCount = 0;
		}
		updateCount++;
		// }
	}

	private void setUncertainty() {
		float guessAge = Random.Range(3, 6);
		float guessHeight = Random.Range(3, 12);
		float guessWeight = Random.Range(4, 10);

		ageUncertainty.text = "(± " + guessAge + ")";
		weightUncertainty.text = "(± " + guessWeight + ")";
		heightUncertainty.text = "(± " + guessHeight + ")";
	}




}