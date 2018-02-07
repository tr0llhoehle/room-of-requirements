using System;
using UnityEngine;

public class PersonInfo {
	public String id;
	public int age;
	public string gender;
	public int height;
	public int weight;

	public static PersonInfo createFromJsonString(string jsonString) {
		return JsonUtility.FromJson<PersonInfo> (jsonString);
	}
}