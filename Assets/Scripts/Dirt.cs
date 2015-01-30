using UnityEngine;
using System.Collections;

public class Dirt : MonoBehaviour {
	public float maxSpeed = 5f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay(Collider other) {
		PE_Obj bikePEO = other.GetComponent<PE_Obj> ();
		
		if (bikePEO == null)
			return;

		if (bikePEO.vel.x > maxSpeed) {
			bikePEO.acc.x = -50f;
		}
	}

}
