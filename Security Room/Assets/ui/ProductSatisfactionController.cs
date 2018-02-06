using System;

using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ProductSatisfactionController : MonoBehaviour {
	public Text productionText;
	public SharedInfo sharedInfo;

	private string currentSubjectId = "";

	void Start() {
		productionText.text = Random.Range(80, 100) + "%";
		Update();
	}

	void Update() {
		if (!currentSubjectId.Equals(SharedInfo.subjectId)) {
			productionText.text = Random.Range(80, 100) + "%";
			currentSubjectId = SharedInfo.subjectId;
		}
	}

}