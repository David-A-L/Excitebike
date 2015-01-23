using UnityEngine;
using System.Collections;

public class BoostScript : MonoBehaviour {
	public GameObject boostGO;
	public float power = 100f;
	Vector3 dir = Vector3.zero;

	void Start(){
		dir = boostGO.transform.right;
	}

	void OnTriggerStay(Collider other){
		//TODO: change direction/power based on input
		if (other.gameObject.tag != "Bike")
			return;
		PE_Obj otherPEO = other.GetComponent<PE_Obj> ();
		otherPEO.acc += dir * power;
	}

}
