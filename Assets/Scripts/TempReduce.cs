using UnityEngine;
using System.Collections;

public class TempReduce : MonoBehaviour {
	public GameObject scriptParent;
	public GameRunner grScript;
	// Use this for initialization
	void Start () {
		scriptParent = GameObject.Find("Main Camera");
		grScript = scriptParent.GetComponent<GameRunner> ();
	}
	
    void OnTriggerEnter(Collider other){
		if (other.tag == "Bike") {
			grScript.reduceTemp();
		}
	}
}
