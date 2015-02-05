using UnityEngine;
using System.Collections;

public class ShadowScript : MonoBehaviour {

	public GameObject bike;
	
	// Update is called once per frame
	void Update () {
		Vector3 t = transform.position;
		t.x = bike.transform.position.x;
		t.z = bike.transform.position.z;
		transform.position = t;
	}
}
