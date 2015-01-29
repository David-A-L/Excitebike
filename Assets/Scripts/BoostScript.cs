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

	void OnTriggerExit(Collider other){
		if (other.gameObject.tag != "Bike")
			return;

		PE_Obj otherPEO = other.GetComponent<PE_Obj> ();

		//if moving too slow, no boost applied
		if (otherPEO.vel.magnitude < minVel)
			return;
		//if holding left, boost will be applied as upward lift
		if (Input.GetKey (KeyCode.LeftArrow) || Input.GetKey (KeyCode.A))
			otherPEO.vel.y += otherPEO.vel.magnitude * power;
		else 
			otherPEO.vel += otherPEO.vel * power;
	}

}
