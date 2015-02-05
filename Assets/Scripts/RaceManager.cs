using UnityEngine;
using System.Collections;

public class RaceManager : MonoBehaviour {
	public Color guiColor;
	public int lapCount = 1;
	public int lapsInRace = 2;
	public float countDown = 10f;
	float curTime;
	bool runStart = false;
	bool runFinish = false;
	public GameObject playerBike;
	// Use this for initialization
	void Start () {
		curTime = Time.time;
		runStart = true;
	}

	void OnGUI () {
		if (runFinish) {
			GUI.contentColor = guiColor;
			GUI.Label (new Rect (450, 50, 200, 2000), "Race Finished!"); 
		}
	}


	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.R)){
			Application.LoadLevel("_scene_0");
		}
		else if (Input.GetKey (KeyCode.T)) {
			Application.LoadLevel ("_scene_1");
		}
		if (runStart)
			StartRace ();
		if (runFinish)
			FinishRace ();
	}

	void OnTriggerEnter(Collider other){
		if (other.tag != "Bike")
			return;
		lapCount++;
		if (lapCount > lapsInRace) {
			curTime = Time.time;
			runFinish = true;
		}
	}

	void StartRace(){

		if (Time.time < curTime + countDown) {
			return;
		}
		runStart = false;
		playerBike.GetComponent<Bike> ().raceStarted = true;
	}

	void FinishRace(){
		if (Time.time < curTime + countDown)
			return;
		runFinish = false;
		//print("RACE FINISHED");
		Application.LoadLevel("_scene_0");
	}
}
