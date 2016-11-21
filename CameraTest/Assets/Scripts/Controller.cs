using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Networking;
using UnityEngine.UI;
using System.Collections;


public class Controller : MonoBehaviour {

	public Text heartbeat;
	public Text subtitle;
	public Canvas mainCanvas;
	public Canvas leftCanvas;
	public Canvas rightCanvas;

	// Use this for initialization
	void Start () {
		StartCoroutine("GetSensorData");

		LoadScene ("testobjects");
		LoadScene ("testobjects2");
	}

	// Get SensorData from server
	IEnumerator GetSensorData() {
		while (true) {
			using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get ("http://localhost/sensordata")) {
				yield return www.Send ();

				if (www.isError) {
					Debug.Log (www.error);
				} else {
					Debug.Log (www.downloadHandler.text);

					SensorData sensors = JsonUtility.FromJson<SensorData> (www.downloadHandler.text);

					heartbeat.text = "Heartbeat: " + sensors.heartbeat.ToString();

					if (sensors.heartbeat == 80) {
						LoadScene ("testobjects");
					} else {
						UnloadScene ("testobjects");
					}
				}
			}
			yield return new WaitForSeconds (1.0f);
		}
	}

	void LoadScene(string name) {
		if (!SceneManager.GetSceneByName (name).isLoaded) {
			SceneManager.LoadScene (name, LoadSceneMode.Additive);
		}
	}

	void UnloadScene(string name) {
		SceneManager.UnloadScene (name);
	}

	void ToggleScene(string name) {
		if (!SceneManager.GetSceneByName (name).isLoaded) {
			SceneManager.LoadScene (name, LoadSceneMode.Additive);
		} else {
			SceneManager.UnloadScene (name);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("a")) {
			if (!SceneManager.GetSceneByName ("testobjects").isLoaded) {
				SceneManager.LoadScene ("testobjects", LoadSceneMode.Additive);
			} else {
				SceneManager.UnloadScene ("testobjects");
			}
		}

		if (Input.GetKeyDown ("d")) {
			SceneManager.UnloadScene ("testobjects");
		}

		if (Input.GetKeyDown ("1")) {
			LoadScene ("testobjects2");
		}

		if (Input.GetKeyDown ("2")) {
			UnloadScene ("testobjects2");
		}
	}
}
