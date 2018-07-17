using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TraitsChartController : MonoBehaviour {
	public RadarChart personalityChart;

	private SharedInfo sharedInfo;
	private string currentSubjectId = "";
	private List<string> savedTraits = new List<string>();

	public void Start() {
		
		if (Dummy.ENABLED) {
			ColorPersonality colorPersonality = Dummy.getDummyColorPersonality();
			setProContraList(colorPersonality);
			setAdditionalTraits(colorPersonality);
		} else {
			for (int i = 0; i < personalityChart.GetParameters().Count; i++) {
				personalityChart.SetParameter(i, 0);
			}
			StartCoroutine(updateTexts());
		}

	}

	// public void Update() {
	// 	for (int i = 0; i < personalityChart.GetParameters().Count; i++) {
	// 		personalityChart.SetParameter(i, Random.Range(0.0f, 1.0f));

	// 	}
	// }

	IEnumerator updateTexts() {
		while (true) {
			WWW www = new WWW(Utility.SUBJECT_URL);
			yield return www;
			//if (www.error == null) {
			string jsonString = www.text;
			ColorPersonality colorPersonality = ColorPersonality.createFromJsonString(jsonString);
			//Debug.Log("ColorPersonality: " + colorPersonality.additional_traits.strength.Length);
			if (!currentSubjectId.Equals(SharedInfo.subjectId)) {
				savedTraits = new List<string>();
				setAdditionalTraits(colorPersonality);
			} else {
				addAdditionalTraits(colorPersonality);
			}
			setProContraList(colorPersonality);
			//} else {
				//print("color personality url not reachable: " + Utility.SUBJECT_URL);
			//}

			yield return new WaitForSeconds(Utility.UPDATE_INTERVAL);
		}
	}

	private void setProContraList(ColorPersonality personality) {
		List<string> pros = new List<string>();
		List<string> cons = new List<string>();

		if (personality.color_traits == null)
			return;

		foreach (ProContraColorTraits proContra in personality.color_traits) {
			foreach (string pro in proContra.pro) {
				if (!string.IsNullOrEmpty(pro) && !pros.Exists(content => content.Equals(pro))) {
					pros.Add(" - " + pro);
					updateChart(PersonalityGeneralizer.getPersonality(pro));
				}
			}
			foreach (string con in proContra.contra) {
				if (!string.IsNullOrEmpty(con) && !cons.Exists(content => content.Equals(con))) {
					cons.Add(" - " + con);
					updateChart(PersonalityGeneralizer.getPersonality(con));
				}
			}
		}

	}

	private void setAdditionalTraits(ColorPersonality colorPersonality) {
		HashSet<string> traits = new HashSet<string>();
		AdditionalColorTraits additionalTraits = colorPersonality.additional_traits;
		if (additionalTraits == null)
			return;

		if (additionalTraits.strength != null) {
			foreach (string trait in additionalTraits.strength) {
				addTrait(ref traits, trait);
			}
		}
		if (additionalTraits.weakness != null) {
			foreach (string trait in additionalTraits.weakness) {
				addTrait(ref traits, trait);
			}
		}
		if (additionalTraits.likes != null) {
			foreach (string trait in additionalTraits.likes) {
				addTrait(ref traits, trait);
			}
		}
		if (additionalTraits.dislikes != null) {
			foreach (string trait in additionalTraits.dislikes) {
				addTrait(ref traits, trait);
			}
		}
		if (additionalTraits.traits != null) {
			foreach (string trait in additionalTraits.traits) {
				addTrait(ref traits, trait);
			}
		}
		string[] traitsArray = new string[traits.Count];

		traits.CopyTo(traitsArray);
		savedTraits.AddRange(traitsArray);

		setTextFromSavedTraits();
	}

	private void addAdditionalTraits(ColorPersonality colorPersonality) {
		AdditionalColorTraits additionalTraits = colorPersonality.additional_traits;
		if (additionalTraits == null)
			return;

		if (additionalTraits.strength != null) {
			foreach (string trait in additionalTraits.strength) {
				addTrait(ref savedTraits, trait);
			}
		}
		if (additionalTraits.weakness != null) {
			foreach (string trait in additionalTraits.weakness) {
				addTrait(ref savedTraits, trait);
			}
		}
		if (additionalTraits.likes != null) {
			foreach (string trait in additionalTraits.likes) {
				addTrait(ref savedTraits, trait);
			}
		}
		if (additionalTraits.dislikes != null) {
			foreach (string trait in additionalTraits.dislikes) {
				addTrait(ref savedTraits, trait);
			}
		}
		if (additionalTraits.traits != null) {
			foreach (string trait in additionalTraits.traits) {
				addTrait(ref savedTraits, trait);
			}
		}

		setTextFromSavedTraits();
	}

	private void addTrait(ref HashSet<string> set, string trait) {
		if (!string.IsNullOrEmpty(trait)) {
			updateChart(PersonalityGeneralizer.getPersonality(trait));
			set.Add(trait);
		}
	}
	private void addTrait(ref List<string> list, string trait) {
		if (!string.IsNullOrEmpty(trait)) {
			updateChart(PersonalityGeneralizer.getPersonality(trait));
			list.Add(trait);
		}
	}

	private void updateChart(Personality personality) {
		int index = -1;
		switch (personality) {
			case Personality.OPENNESS:
				index = 0;
				break;
			case Personality.EXTRAVERSION:
				index = 1;
				break;
			case Personality.CONSCIENTIOUS:
				index = 2;
				break;
			case Personality.NEUROTICISM:
				index = 3;
				break;
			case Personality.AGREEABLENESS:
				index = 4;
				break;
		}
		int factor = 1;
		float chanceForNegative = 0.1f;
		if (Random.Range(0.0f, 1.0f) < chanceForNegative) {
			factor = -1;
		}
		float defaultUpdateStep = 0.75f * factor;
		float oldParam = personalityChart.GetParameter(index);
		float newParam = oldParam + defaultUpdateStep;
		if (newParam >= 1) {
			newParam = 1;
		} else if (newParam <= 0) {
			newParam = 0;
		}
		personalityChart.SetParameter(index, newParam);

	}

	private void setTextFromSavedTraits() {
		List<string> toShow = new List<string>();
		if (savedTraits.Count > 12) {
			toShow.AddRange(savedTraits.GetRange(savedTraits.Count - 12, 12));
		} else {
			toShow.AddRange(savedTraits);
		}

	}
}