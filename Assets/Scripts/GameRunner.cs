using UnityEngine;
using System.Collections;

public class GameRunner : MonoBehaviour {

	public float temp;
	public float restingTemp = 10f;
	public float mediumTemp = 60f;
	public float maxTemp = 100f;
	public Bike bikeScript;
	public GameObject playerBike;
	// Use this for initialization
	void Start () {
		
		temp = 10f;
		bikeScript = playerBike.GetComponent<Bike> ();
	}
	
	// Update is called once per frame
	void Update () {
		
		if (temp > 100f) {
			//bikeScript.overheat ();
		}
	}
}
