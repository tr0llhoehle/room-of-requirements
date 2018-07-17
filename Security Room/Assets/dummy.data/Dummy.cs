using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dummy {
	public static readonly bool ENABLED = true;

	public static ColorPersonality getDummyColorPersonality() {
		ColorPersonality personality = new ColorPersonality();

		personality.additional_traits = getDummyAdditionalColorTraits();
		personality.color_traits = new ProContraColorTraits[] {getDummyProContraColorTraits1(), getDummyProContraColorTraits2()};

		return personality;
	}

	public static AdditionalColorTraits getDummyAdditionalColorTraits() {
		return new AdditionalColorTraits {
				strength = new string[] { "strength", "strength" },
				weakness = new string[] {"weakness1","weakness2" },
				likes = new string[] { "likes1","likes2","likes3"},
				dislikes = new string[] { "dislikes1","dislikes2"},
				traits = new string[] {"trait1" },
		};
	}

	public static ProContraColorTraits getDummyProContraColorTraits1() {
		return new ProContraColorTraits {
				pro = new string[] { "pro1", "pro2" },
				contra = new string[] {"con1","con2", "con3" },
		};
	}

	public static ProContraColorTraits getDummyProContraColorTraits2() {
		return new ProContraColorTraits {
				pro = new string[] { "pro5", "pro8", "pro2" },
				contra = new string[] {"con4","con10" },
		};
	}

	public static PersonInfo getDummyPersonInfo() {
		return new PersonInfo {
			id = "22354",
			age = 21,
			gender = "female",
			height = 1.55,
			weight = 50
		};
	}

	public static GeneralizedPersonality getDummyRandomizedGeneralizedPersonality() {
		return new GeneralizedPersonality {
			neuroticism = Random.Range(0f, 100f),
			openness = Random.Range(0f, 100f),
			conscientiousness = Random.Range(0f, 100f),
			agreeableness = Random.Range(0f, 100f),
			extroversion = Random.Range(0f, 100f)
		};
	}

}