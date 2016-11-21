using UnityEngine;
using System.Collections;

public class MoveCube : MonoBehaviour {

	public float _Angle;
	public float _Period;
	private float time;
	
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		// rotate beween -_Angle and +_Angle
		time = time + Time.deltaTime;
		float phase = Mathf.Sin(time / _Period);
		transform.localRotation = Quaternion.Euler( new Vector3(0, phase * _Angle, 0));
	}
}
