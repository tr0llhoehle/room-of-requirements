using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendMove : MonoBehaviour {
    private Renderer renderer;

    private TextMesh debugText = null;
    private float balance = 0.5f;
    private float balanceUpdate = 0.01f;

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
        if (GameModel.Instance.faceData != null)
        {
            if (GameModel.Instance.faceData.yaw < 0)
                balance += balanceUpdate * Mathf.Abs(GameModel.Instance.faceData.yaw / 45.0f);
            if (GameModel.Instance.faceData.yaw > 0)
                balance -= balanceUpdate * Mathf.Abs(GameModel.Instance.faceData.yaw / 45.0f);
            balance = Mathf.Max(0.0f, Mathf.Min(1.0f, balance));
            Debug(string.Format("Balance: {0} Yaw: {1}", balance, GameModel.Instance.faceData.yaw));
            renderer.material.SetFloat("_Balance", balance);
        }
	}
}
