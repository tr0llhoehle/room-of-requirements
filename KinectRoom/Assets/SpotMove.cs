﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotMove : MonoBehaviour {
	Transform spottrans;
	float angle = -120;

    // Use this for initialization
    void Start () {
		spottrans = GameObject.Find ("Spotlight").GetComponent<Transform>();
	}

    // Update is called once per frame
    void Update() {
        /*
        angle += 1;
		if (angle > 120)
			angle = -120;
		*/
        if (GameModel.Instance.faceData != null)
        {
            angle = GameModel.Instance.faceData.yaw;
            spottrans.eulerAngles = new Vector3(0, angle*-3, 0);
        }
	}
}
