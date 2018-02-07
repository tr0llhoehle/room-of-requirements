using UnityEngine;
using System.Collections;

public class Utility {
	private static readonly string BASE_PATH = "http://127.0.0.1:3000/";
	public static readonly string IMAGE_URL = BASE_PATH + "current_image";
	public static readonly string SUBJECT_URL = BASE_PATH + "current_subject";
	// public static readonly string PERSONALITY_URL = BASE_PATH + "personality";
	// public static readonly string GENERAL_INFO_URL = BASE_PATH + "personality";
	// public static readonly string COLOR_TRAITS_URL = BASE_PATH + "personality";
	//in seconds
	public static readonly float UPDATE_INTERVAL = 0.5f;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

