using System;
using UnityEngine;

public class GeneralizedPersonality {
	public float neuroticism;
	public float openness;
	public float conscientiousness;
	public float agreeableness;
	public float extroversion;

	public static GeneralizedPersonality createFromJsonString(string jsonString) {
		return JsonUtility.FromJson<GeneralizedPersonality>(jsonString);
	}
}