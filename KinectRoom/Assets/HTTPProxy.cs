using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HTTPProxy : MonoBehaviour {
    public string url = "http://127.0.0.1:3000/kinect";
    public float updateSeconds = 0.1f;

    private IEnumerator updater;

    IEnumerator UpdateState()
    {
        while (true)
        {
            WWW www = new WWW(url);
            yield return www;

            var data = FaceData.CreateFromJSON(www.text);
            if (GameModel.Instance.faceData == null || data.time > GameModel.Instance.faceData.time)
            {
                Debug.Log(string.Format("Time {0}", data.time));
                GameModel.Instance.faceData = data;
            }
            yield return new WaitForSeconds(updateSeconds);
        }
    }

    // Use this for initialization
    void Start () {
        updater = UpdateState();
        StartCoroutine(updater);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
