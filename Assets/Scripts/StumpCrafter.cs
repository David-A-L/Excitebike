//move stumps further back in z direction
using UnityEngine;
using System.Collections;

public class StumpCrafter : MonoBehaviour {
	//fields set in the Unity Inspector pane
	public GameObject stumpPrefab;
	public bool _____________________;

	//fields set dynamically
	public GameObject[] stumpInstances;
	int numStumps = 1000;

	void Awake () {
		//make an array to hold all the stump instances
		stumpInstances = new GameObject[numStumps];
		//find StumpAnchor parent GameObject
		GameObject anchor = GameObject.Find ("StumpAnchor");
		//position of first stump
		Vector3 sPos = Vector3.zero;
		sPos.x = -350.8f;
		sPos.y = -3f;
		sPos.z = 5f;
		//iterate through and create stumps
		numStumps = 1000;
		GameObject stump;
		for (int i = 0; i < numStumps; i++){
			stump = Instantiate (stumpPrefab) as GameObject;
			//position stump
			sPos.x += .8f;
			//apply position to stump
			stump.transform.position = sPos;
			//make stump a child of anchor
			stump.transform.parent = anchor.transform;
			//add the stump to stumpInstances
			stumpInstances[i] = stump;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
