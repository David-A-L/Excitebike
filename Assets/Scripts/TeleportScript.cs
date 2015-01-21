using UnityEngine;
using System.Collections;

public class TeleportScript : MonoBehaviour {
	public GameObject tpExit;
	// Use this for initialization
	void Start () {
		tpExit = GameObject.Find("TPExit");
	}
	
	void OnTriggerEnter (Collider other){
		Vector3 temp = other.transform.position;
		temp.x = tpExit.transform.position.x;
		other.transform.position = temp;
	}
}
