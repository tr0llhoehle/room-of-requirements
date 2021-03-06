﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;



public class HTTPProxy : MonoBehaviour {
    public string url = "http://127.0.0.1:3000";
    public float updateFaceSeconds = 0.1f;
    public float updateGestureSeconds = 0.5f;
    public float sendSeconds = 1.0f;

    private string faceUrl;
    private string gestureUrl;
    private string colorUrl;

    private VideoPlayer cold_player;
    private VideoPlayer warm_player;


    HTTPProxy()
    {
        faceUrl = url + "/face";
        gestureUrl = url + "/gesture";
        colorUrl = url + "/colors";
    }

    private IEnumerator faceUpdater;
    private IEnumerator gestureUpdater;

    private IEnumerator sender;

    IEnumerator SendState()
    {
        while (true)
        {
            if (GameModel.Instance.faceData != null)
            {
                var postHeader = new Dictionary<string, string>();
                postHeader.Add("Content-Type", "application/json");

                // FIXME check if this is inverted
                var wall = GameModel.Instance.faceData.yaw > 0 ? "warm" : "cold";
                var time = GameModel.Instance.faceData.yaw > 0 ? warm_player.time : cold_player.time;
                var jsonData = string.Format("{{\"time\": {0}, \"wall\": \"{1}\" }}", time, wall);
                var raw_data = System.Text.Encoding.UTF8.GetBytes(jsonData);

                WWW www = new WWW(colorUrl, raw_data, postHeader);
                yield return www;
            }

            yield return new WaitForSeconds(sendSeconds);
        }
    }

    IEnumerator UpdateFace()
    {
        while (true)
        {
            WWW www = new WWW(faceUrl);
            yield return www;

            var data = FaceData.CreateFromJSON(www.text);
            if (data != null)
            {
                if (GameModel.Instance.faceData == null || data.time > GameModel.Instance.faceData.time)
                {
                    Debug.Log(string.Format("Face data time {0}", data.time));
                    GameModel.Instance.faceData = data;
                }
            }

            www = new WWW(gestureUrl);
            yield return www;

            var gestureData = GestureData.CreateFromJSON(www.text);
            if (gestureData != null)
            {
                if (GameModel.Instance.gestureData == null || gestureData.time > GameModel.Instance.gestureData.time)
                {
                    Debug.Log(string.Format("Gesture data time {0}", gestureData.time));
                    GameModel.Instance.gestureData = gestureData;
                }
            }

            yield return new WaitForSeconds(updateFaceSeconds);
        }
    }

    IEnumerator UpdateGesture()
    {
        while (true)
        {
            WWW www = new WWW(gestureUrl);
            yield return www;

            var gestureData = GestureData.CreateFromJSON(www.text);
            if (gestureData != null)
            {
                if (GameModel.Instance.gestureData == null || gestureData.time > GameModel.Instance.gestureData.time)
                {
                    Debug.Log(string.Format("Gesture data time {0}", gestureData.time));
                    GameModel.Instance.gestureData = gestureData;
                }
            }

            yield return new WaitForSeconds(updateGestureSeconds);
        }
    }

    // Use this for initialization
    void Start () {
        cold_player = GameObject.Find("ColdPlayer").GetComponent<VideoPlayer>();
        warm_player = GameObject.Find("CatPlayer").GetComponent<VideoPlayer>();

        faceUpdater = UpdateFace();
        StartCoroutine(faceUpdater);
        gestureUpdater = UpdateGesture();
        StartCoroutine(gestureUpdater);
        sender = SendState();
        StartCoroutine(sender);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
