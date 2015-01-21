using UnityEngine;
using System.Collections;

public class WallContact : MonoBehaviour {


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//need to make this work with moving down to the lane and reducing accel
	//this is for top wall (tag the walls)
	void OnTriggerStay(Collider other){
		/*Bike bikeScript = other.GetComponent<Bike> ();
		PE_Obj bikePEO = other.GetComponent<PE_Obj> ();

		if (bikeScript.curDirIn != Bike.DirInput.UP) {
			bikePEO.vel.z = 0f;
			//tweak
			bikePEO.acc.x = -3f;
		}*/

		PE_Obj bikePEO = other.GetComponent<PE_Obj> ();

		if (Input.GetKey (KeyCode.UpArrow))
		{
			bikePEO.vel.z = 0f;
			//tweak (add limiter)
			bikePEO.acc.x = -3f;
		}
		

	}

}
