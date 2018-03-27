using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendMove : MonoBehaviour {
    private Renderer frontRenderer;
    private Renderer leftRenderer;
    private Renderer rightRenderer;

    private TextMesh debugText = null;
    private float balance = 1.5f;
    private float balanceUpdate = 0.05f;

    private void Debug(string text)
    {
        //debugText.text = text;
    }
    // Use this for initialization
    void Start () {
        frontRenderer = GameObject.Find("Front Wall").GetComponent<Renderer>();
        leftRenderer = GameObject.Find("Left Wall").GetComponent<Renderer>();
        rightRenderer = GameObject.Find("Right Wall").GetComponent<Renderer>();

        leftRenderer.material.SetFloat("_Balance", 1.0f);
        frontRenderer.material.SetFloat("_Balance", 0.5f);
        rightRenderer.material.SetFloat("_Balance", 0.0f);

        debugText = GameObject.Find("DebugText").GetComponent<TextMesh>();
    }

    float Sigmoid(float x)
    {
        float y = Mathf.Exp(x);
        return y / (1 + y);
    }

    // Update is called once per frame
    void Update () {
        float yaw = 0;
        if (GameModel.Instance.faceData != null)
        {
            yaw = GameModel.Instance.faceData.yaw;
        }
        else
        {
            yaw = Mathf.PingPong(Time.time*10, 90.0f) - 45.0f;
        }

        balance += balanceUpdate * (2.0f*Sigmoid(yaw/3.0f) - 1.0f);

        balance = Mathf.Max(0.0f, Mathf.Min(3.0f, balance));

        Debug(string.Format("Balance: {0} Yaw: {1}", balance, yaw));   

        float leftBalance = balance;
        float frontBalance = balance - 1.0f;
        float rightBalance = balance - 2.0f;

        leftRenderer.material.SetFloat("_Balance", leftBalance);
        frontRenderer.material.SetFloat("_Balance", frontBalance);
        rightRenderer.material.SetFloat("_Balance", rightBalance);
	}
}
