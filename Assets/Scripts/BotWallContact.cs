using UnityEngine;
using System.Collections;

public class BotWallContact : MonoBehaviour {

	float wallSpeed = 5f;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay(Collider other){
		PE_Obj bikePEO = other.GetComponent<PE_Obj> ();
		
		if (Input.GetKey (KeyCode.DownArrow) && other.tag == "Bike")
		{
			bikePEO.vel.z = 0f;
			//tweak (add limiter)
			if (bikePEO.vel.x > wallSpeed) {
				bikePEO.acc.x = -3f;
			}
			//update these values if we tweak bike.cs
			else if (bikePEO.vel.x < wallSpeed) {
				if (Input.GetKey (KeyCode.X)) {
					bikePEO.acc.x = 3.25f;
				}
				else if (Input.GetKey (KeyCode.Z)) {
					bikePEO.acc.x = 5f;
				}
			}
			else {
				bikePEO.acc.x = 0f;
			}
		}
		
		
	}
	
}
