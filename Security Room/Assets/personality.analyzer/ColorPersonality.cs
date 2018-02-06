using System;
using UnityEngine;

public class ColorPersonality {
	public ProContraColorTraits[] color_traits;
	public AdditionalColorTraits additional_traits;

	public static ColorPersonality createFromJsonString(string jsonString) {
		return JsonUtility.FromJson<ColorPersonality> (jsonString);
	}
}