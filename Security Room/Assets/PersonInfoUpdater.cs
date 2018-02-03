using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonInfoUpdater : MonoBehaviour {
	public Text subjectId;
	public Text age;
	public Text gender;
	public Text height;
	public Text weight;
	// Use this for initialization
	void Start () {
		StartCoroutine(updateBars());
	}

	IEnumerator updateBars() {
		while (true) {
			WWW www = new WWW (Utility.PERSONALITY_URL);
			yield return www;
			if (www.error == null) {
				string jsonString = www.text;
				PersonInfo personInfo = PersonInfo.createFromJsonString (jsonString);
				subjectId.text = personInfo.id;
				age.text = personInfo.age;
				gender.text = personInfo.gender;
				height.text = personInfo.height;
				weight.text = personInfo.weight;
			} else {
				print ("personality url not reachable: " + Utility.PERSONALITY_URL);
			}

			print ("waiting");
			yield return new WaitForSeconds (Utility.UPDATE_INTERVAL);
			print ("waited");

		}
	}
	
}
