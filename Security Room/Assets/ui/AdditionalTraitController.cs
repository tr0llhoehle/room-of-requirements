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
		//if (Dummy.ENABLED) {
		//	ColorPersonality colorPersonality = Dummy.getDummyColorPersonality();
		//	setProContraList(colorPersonality);
		//	setAdditionalTraits(colorPersonality);
		//} else {
			StartCoroutine(updateTexts());
		//}

	}

	IEnumerator updateTexts() {
		while (true) {
			WWW www = new WWW(Utility.SUBJECT_URL);
			yield return www;
			//if (www.error == null) {
				string jsonString = www.text;
				ColorPersonality colorPersonality = ColorPersonality.createFromJsonString(jsonString);
				if (!currentSubjectId.Equals(SharedInfo.subjectId)) {
					savedTraits = new List<string>();
					setAdditionalTraits(colorPersonality);
				} else {
					addAdditionalTraits(colorPersonality);
				}
				setProContraList(colorPersonality);
			//} else {
			//	print("color personality url not reachable: " + Utility.SUBJECT_URL);
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
				if (!pros.Exists(content => content.Equals(pro))) {
					pros.Add(" - " + pro);
				}
			}
			foreach (string con in proContra.contra) {
				if (!cons.Exists(content => content.Equals(con))) {
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

        if (additionalTraits.strength != null)
            foreach (string trait in additionalTraits.strength) {
			traits.Add(trait);
		}
        if (additionalTraits.weakness != null)
            foreach (string trait in additionalTraits.weakness) {
			traits.Add(trait);
		}
        if (additionalTraits.likes != null)
            foreach (string trait in additionalTraits.likes) {
			traits.Add(trait);
		}
        if (additionalTraits.dislikes != null)
            foreach (string trait in additionalTraits.dislikes) {
			traits.Add(trait);
		}
        if (additionalTraits.traits != null)
            foreach (string trait in additionalTraits.traits) {
			traits.Add(trait);
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

        if (additionalTraits.strength != null)
		foreach (string trait in additionalTraits.strength) {
			savedTraits.Add(trait);
		}
        if (additionalTraits.weakness != null)
        foreach (string trait in additionalTraits.weakness) {
			savedTraits.Add(trait);
		}
        if (additionalTraits.likes != null)
            foreach (string trait in additionalTraits.likes) {
			savedTraits.Add(trait);
		}
        if (additionalTraits.dislikes != null)
            foreach (string trait in additionalTraits.dislikes) {
			savedTraits.Add(trait);
		}
        if (additionalTraits.traits != null)
            foreach (string trait in additionalTraits.traits) {
			savedTraits.Add(trait);
		}

		setTextFromSavedTraits();
	}

	private void setTextFromSavedTraits() {
		List<string> toShow = new List<string>();
		if (savedTraits.Count > 12) {
			toShow.AddRange(savedTraits.GetRange(savedTraits.Count - 12, 12));
		} else {
			toShow.AddRange(savedTraits);
		}

		traitsList.text = string.Join("\n ", toShow.ToArray());
	}
}