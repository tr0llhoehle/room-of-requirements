using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdditionalTraitController : MonoBehaviour {
	public Text prosList;
	public Text consList;

	public Text traitsList;

	private SharedInfo sharedInfo;
	private string currentSubjectId = "";
	private List<string> savedTraits = new List<string>();

	public void Start() {
		if (Dummy.ENABLED) {
			ColorPersonality colorPersonality = Dummy.getDummyColorPersonality();
			setProContraList(colorPersonality);
			setAdditionalTraits(colorPersonality);
		} else {
			StartCoroutine(updateTexts());
		}

	}

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
				}
			}
			foreach (string con in proContra.contra) {
				if (!string.IsNullOrEmpty(con) && !cons.Exists(content => content.Equals(con))) {
					cons.Add(" - " + con);
				}
			}
		}

		prosList.text = String.Join("\n", pros.ToArray());
		consList.text = String.Join("\n", cons.ToArray());
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
			set.Add(trait);
		}
	}
	private void addTrait(ref List<string> list, string trait) {
		if (!string.IsNullOrEmpty(trait)) {
			list.Add(trait);
		}
	}

	private void setTextFromSavedTraits() {
		List<string> toShow = new List<string>();
		if (savedTraits.Count > 12) {
			toShow.AddRange(savedTraits.GetRange(savedTraits.Count - 12, 12));
		} else {
			toShow.AddRange(savedTraits);
		}

		traitsList.text = string.Join("\n", toShow.ToArray());
	}
}