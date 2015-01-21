using UnityEngine;
using System.Collections;

public class CameraLock : MonoBehaviour {
	//public int	dist = 10;
	// Use this for initialization

	public Transform	target;
	public Vector3		offset;

	void Start () {
		offset = this.transform.position - target.position;
	}

	//var target : Transform;
	//var distance : Float;
	// Update is called once per frame
	void Update () {
		//camera.main.transform.position.z = 0;

		//transform.position.z = target.position.z -distance;
		//transform.position.y = target.position.y;
		//transform.position.x = target.position.x;

		//int DistanceAway = 10;
		//Vector3 bikePos = GameObject.Find("Bike").transform.position;
		//GameObject.Find ("MainCamera").transform.position = new Vector3 (bikePos.x, bikePos.y, bikePos.z - dist);
	}

	void FixedUpdate() {
		Vector3 temp = transform.position;
		temp.x = target.position.x + offset.x;
		transform.position = temp;
	}
	//this is glitchy, talk in office hours about how to fix this by using a script and also not having
	//	main camera be a child of bike
	void LateUpdate () {

		/*
		Vector3 tempPos = transform.position;
		tempPos.z = -6.07f;
		tempPos.y = 2.75f;
		transform.position = tempPos;

		Vector3 tempRot = new Vector3 (37.68628f, 0f, 0f);

		transform.rotation = Quaternion.Euler(tempRot);
		*/
		//transform.rotation = Quaternion.Euler(Vector3(0, 0, 0));  // 90 degress on the X axis - change appropriately*/
	}
}
