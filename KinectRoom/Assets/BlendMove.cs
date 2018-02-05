using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendMove : MonoBehaviour {
    private Renderer renderer;

    private TextMesh debugText = null;


    private void Debug(string text)
    {
        debugText.text = text;
    }
    // Use this for initialization
    void Start () {
        renderer = GetComponent<Renderer>();
        debugText = GameObject.Find("DebugText").GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update () {
        float balance = 1.0f;
        if (GameModel.Instance.faceData != null)
        {
            balance = Mathf.Min(1.0f, Mathf.Max(-1.0f, GameModel.Instance.faceData.yaw / -50.0f)) * 0.5f + 0.5f;
            Debug(string.Format("Balance: {0} Yaw: {1}", balance, GameModel.Instance.faceData.yaw));
            renderer.material.SetFloat("_Balance", balance);
        }
	}
}
