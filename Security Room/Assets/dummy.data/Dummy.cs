using System;

public class Dummy {

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

}