using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdditionalTraitController : MonoBehaviour {
	public Text prosList;
	public Text consList;

	public Text traitsList;

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
			WWW www = new WWW(Utility.COLOR_TRAITS_URL);
			yield return www;
			if (www.error == null) {
				string jsonString = www.text;
				ColorPersonality colorPersonality = ColorPersonality.createFromJsonString(jsonString);
				setProContraList(colorPersonality);
				setAdditionalTraits(colorPersonality);
			} else {
				print("color personality url not reachable: " + Utility.COLOR_TRAITS_URL);
			}

			yield return new WaitForSeconds(Utility.UPDATE_INTERVAL);
		}
	}

	private void setProContraList(ColorPersonality personality) {
		List<string> pros = new List<string>();
		List<string> cons = new List<string>();

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
		foreach (string trait in additionalTraits.strength) {
			traits.Add(trait);
		}
		foreach (string trait in additionalTraits.weakness) {
			traits.Add(trait);
		}
		foreach (string trait in additionalTraits.likes) {
			traits.Add(trait);
		}
		foreach (string trait in additionalTraits.dislikes) {
			traits.Add(trait);
		}
		foreach (string trait in additionalTraits.traits) {
			traits.Add(trait);
		}
	}
}