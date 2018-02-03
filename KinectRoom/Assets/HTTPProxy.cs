using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HTTPProxy : MonoBehaviour {
    public string url = "http://127.0.0.1:3000";
    public float updateSeconds = 0.01f;

    private string faceUrl;
    private string gestureUrl;

    HTTPProxy()
    {
        faceUrl = url + "/face";
        gestureUrl = url + "/gesture";
    }

    private IEnumerator updater;

    IEnumerator UpdateState()
    {
        while (true)
        {
            WWW www = new WWW(faceUrl);
            yield return www;

            var data = FaceData.CreateFromJSON(www.text);
            if (GameModel.Instance.faceData == null || data.time > GameModel.Instance.faceData.time)
            {
                //Debug.Log(string.Format("Face data time {0}", data.time));
                GameModel.Instance.faceData = data;
            }

            www = new WWW(gestureUrl);
            yield return www;

            var gestureData = GestureData.CreateFromJSON(www.text);
            //if (GameModel.Instance.gestureData != null)
            //Debug.Log(string.Format("Gesture data time {0}, {1}", gestureData.time, GameModel.Instance.gestureData.time));

            if (GameModel.Instance.gestureData == null || gestureData.time > GameModel.Instance.gestureData.time)
            {
                //Debug.Log(string.Format("Gesture data time {0}", gestureData.time));
                GameModel.Instance.gestureData = gestureData;
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
