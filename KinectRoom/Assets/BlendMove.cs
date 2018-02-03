using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendMove : MonoBehaviour {
    private Renderer renderer;

	// Use this for initialization
	void Start () {
        renderer = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
        float balance = 1.0f;
        if (GameModel.Instance.faceData != null)
        {
            balance = Mathf.Max(0.0f, Mathf.Min(1.0f, GameModel.Instance.faceData.yaw / -45.0f) + 1.0f);
            renderer.material.SetFloat("_Balance", balance);
        }
	}
}
