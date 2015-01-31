using UnityEngine;
using System.Collections;

public class GameRunner : MonoBehaviour {

	public int temp;
	public int restingTemp = 100;
	public int mediumTemp = 600;
	public int maxTemp = 1000;
	public int cooling = -1;
	public int tempReduce = -500;
	public int fastHeat = 2;
	public int slowHeat = 1;
	public Bike bikeScript;
	public GameObject playerBike;
	public GameObject tempMeter;
	// Use this for initialization
	void Start () {
		
		temp = restingTemp;
		bikeScript = playerBike.GetComponent<Bike> ();
	}
	

	void FixedUpdate () {
		if (bikeScript.crashed) {
			temp = (temp+cooling < restingTemp)? restingTemp: temp+cooling;
		}
		else if (bikeScript.overheated){
			temp = restingTemp;
		}
		else {
			int goalTemp = restingTemp;
			int tempChange = 0;
			if (temp >= maxTemp) {
				bikeScript.overheated = true;
				bikeScript.curTime = Time.time;
				//bikeScript.overheat ();
			}
			else if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Comma)){
				goalTemp = maxTemp + 10;
				tempChange = fastHeat;
			}
			else if (Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.Period)){
				goalTemp = mediumTemp;
				tempChange = slowHeat;
			}
			else {
				goalTemp = restingTemp;
			}

			if (temp > goalTemp){
				temp = (temp+cooling < goalTemp)? goalTemp: temp+cooling;
			}
			else{
				temp = (temp+tempChange < goalTemp)? temp + tempChange: goalTemp;
			}
		}
		//print (temp);
		//anchor box
		//make a red box, correspond the size of the box to temp
		tempMeter = GameObject.Find ("TempBar");
		Vector3 tempSize = Vector3.zero;
		tempSize.x = temp / 100;
		tempSize.y = .01f;
		tempSize.z = 2.139454f;
		print (tempSize.x);
		print (tempSize.y);
		print (tempSize.z);
		//print (tempSize.x);
		tempMeter.transform.localScale = tempSize;

	}
	

	public void reduceTemp(){
		temp += tempReduce;
		temp = temp > restingTemp ? temp : restingTemp; 
	}
}