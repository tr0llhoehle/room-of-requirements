using UnityEngine;

[System.Serializable]
public class SensorData
{
	public int heartbeat;

	public static SensorData CreateFromJSON(string jsonString)
	{
		return JsonUtility.FromJson<SensorData>(jsonString);
	}

	// Given JSON input:
	// {"heartbeat":80}
	// this example will return a SensorData object with
	// heartbeat == 80

}