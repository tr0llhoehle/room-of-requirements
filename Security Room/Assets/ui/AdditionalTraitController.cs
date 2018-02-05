using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdditionalTraitController : MonoBehaviour {
	public Text proList;
	public Text conList;

	public Text strengths;
	public Text weaknesses;
	public Text likes;
	public Text dislikes;
	public Text traits;

	public void Start() {
		ColorPersonality colorPersonality = Dummy.getDummyColorPersonality();
		setProContraList(colorPersonality);
		setAdditionalTraits(colorPersonality);

		// StartCoroutine(updateTexts());
	}

	IEnumerator updateTexts() {
		while (true) {
			WWW www = new WWW(Utility.PERSONALITY_URL);
			yield return www;
			if (www.error == null) {
				string jsonString = www.text;
				ColorPersonality colorPersonality = ColorPersonality.createFromJsonString(jsonString);
				setProContraList(colorPersonality);
				setAdditionalTraits(colorPersonality);
			} else {
				// print("color personality url not reachable: " + Utility.PERSONALITY_URL);
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
					pros.Add(pro);
				}
			}
			foreach (string con in proContra.contra) {
				if (!cons.Exists(content => content.Equals(con))) {
					cons.Add(con);
				}
			}
		}

		proList.text = String.Join("\n", pros.ToArray());
		conList.text = String.Join("\n", cons.ToArray());
	}

	private void setAdditionalTraits(ColorPersonality colorPersonality) {
		AdditionalColorTraits additionalTraits = colorPersonality.additional_traits;
		strengths.text = String.Join(", ", additionalTraits.strength);
		weaknesses.text = String.Join(", ", additionalTraits.weakness);
		likes.text = String.Join(", ", additionalTraits.likes);
		dislikes.text = String.Join(", ", additionalTraits.dislikes);
		traits.text = String.Join(", ", additionalTraits.traits);
	}
}