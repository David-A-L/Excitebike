using UnityEngine;
using System.Collections;

public class CameraLock : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//camera.main.transform.position.z = 0;
	}

	void LateUpdate () {
		Vector3 tempPos = transform.position;
		tempPos.z = -6.07f;
		transform.position = tempPos;
	}
}
