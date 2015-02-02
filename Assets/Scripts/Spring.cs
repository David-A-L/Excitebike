using UnityEngine;
using System.Collections;

public class Spring : MonoBehaviour {
	//public GameObject scriptParent;
	//public GameRunner grScript;
	// Use this for initialization
	void Start () {
		//scriptParent = GameObject.Find("Main Camera");
		//grScript = scriptParent.GetComponent<GameRunner> ();
	}
	
	void OnTriggerEnter(Collider other){
		if (other.tag == "Bike") {
			print ("asdf");
			PE_Obj bikePEO = other.GetComponent<PE_Obj> ();
			Vector3 tempVel = bikePEO.vel;
			tempVel.x = -7f;
			bikePEO.vel = tempVel;
			print (bikePEO.vel.x);
		}
	}
}
