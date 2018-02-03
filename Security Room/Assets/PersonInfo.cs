using System;
using UnityEngine;

public class PersonInfo {
	public String id;
	public String age;
	public String gender;
	public String height;
	public String weight;

	public static PersonInfo createFromJsonString(string jsonString) {
		return JsonUtility.FromJson<PersonInfo> (jsonString);
	}
}