using UnityEngine;

public class PersonalityInfo {
	public float personality1;
	public float personality2;
	public float personality3;
	public float personality4;
	public float personality5;

	public static PersonalityInfo createFromJsonString(string jsonString) {
		return JsonUtility.FromJson<PersonalityInfo> (jsonString);
	}

}

