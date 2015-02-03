using UnityEngine;
using System.Collections;

public class CeilingScript : MonoBehaviour {

	void OnTriggerStay(Collider other){
		if (other.tag != "Bike")
			return;
		PE_Obj bikePEO = other.GetComponent<PE_Obj> ();
		bikePEO.vel.y = 0;
		bikePEO.acc.y = 0;
	}
}
