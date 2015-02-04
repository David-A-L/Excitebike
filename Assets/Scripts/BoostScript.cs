using UnityEngine;
using System.Collections;

public class BoostScript : MonoBehaviour {
	public GameObject boostGO;
	//percentage boost applied to velocity
	public float power = .01f;
	//minimum speed to trigger the vertical boost
	public float minVel = 1.5f;
	//Vector3 dir = Vector3.zero;

	void Start(){
	//	dir = boostGO.transform.right;
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag != "Bike")
			return;

		PE_Obj otherPEO = other.GetComponent<PE_Obj> ();

		if (other.GetComponent<Bike> ().curState == Bike.State.IN_AIR)
			return;

		//no boosting if falling onto the ramp
		if (otherPEO.vel.y < -1f) {
			//print ("negative y");
			return;
		}
		//if moving too slow, no boost applied
		if (otherPEO.vel.y < minVel){
			//print ("too slow");
			return;
		}
		//if holding left, boost will be applied as upward lift
		if (Input.GetKey (KeyCode.LeftArrow) || Input.GetKey (KeyCode.A)) {
			otherPEO.vel.y += otherPEO.vel.x * power;
			otherPEO.vel.x -= otherPEO.vel.x * power/4;
		}
		else {
			otherPEO.vel += boostGO.transform.right * otherPEO.vel.x * power;
			//print ("boosting");
		}
	}
}
