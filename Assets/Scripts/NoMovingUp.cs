using UnityEngine;
using System.Collections;

public class NoMovingUp : MonoBehaviour {
	GameObject playerBike;
	// Use this for initialization
	void Start () {
		playerBike = GameObject.FindGameObjectWithTag ("Bike");
	}
	
	// Update is called once per frame

	void OnTriggerStay(Collider other){

		if (other.tag != "Bike")
			return;
		Bike bScript = playerBike.GetComponent<Bike> ();
		if ( bScript.curDirIn == Bike.DirInput.UP)
			bScript.curDirIn = Bike.DirInput.NONE;
	}
}
