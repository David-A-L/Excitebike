﻿using UnityEngine;
using System.Collections;

public class Bike : MonoBehaviour {
	public Color guiColor;
	public AudioClip idle;
	public AudioClip slowAccelSound;
	public AudioClip fastAccelSound;
	public AudioClip crashSound;
	public AudioClip overheatSound;
	public AudioClip startSound;

	//two kinds of ramp, can move on one but not the other
	public enum State {
		IN_AIR,
		ON_GROUND,
		ON_RAMP
	}

	public enum AccInput {
		NONE,
		SLOW,
		FAST
	}

	public enum DirInput {
		UP,
		DOWN,
		NONE
	}

	public enum RotInput {
		LEFT, //aka rotating counter clockwise
		RIGHT, //aka rotating clockwise
		NONE
	}

	public bool raceStarted = false;

	public bool useJetPack;
	public bool jetOn = false;
	//public int maxFuel = 1000;
	//public int jetFuel;
	//public int consumeRate = -5;
	//public int regenRate = 3;
	public int jetHeatRate = 5;
	public float jetPower = 30f;
	GameObject flames;

	public AccInput curAccIn = AccInput.NONE;
	public DirInput curDirIn = DirInput.NONE;
	public DirInput prevDirIn = DirInput.NONE;
	public RotInput curRotIn = RotInput.NONE;
	public State curState = State.ON_GROUND;

	//max speed is same regardless of fast or slow accel
	public float maxSpeed = 8f;
	public float maxAirSpeed = 8f;
	public float maxAngle = 80f;

	public Vector3 newPosition = Vector3.zero;

	float rotSpeed = 70f;
	public float slowAcc = 3.25f;
	public float fastAcc = 5f;
	float constDecel = -8f;
	float airDecel = -4f;

	float up = 5f;
	float down = -5f;
	float stay = 0f;
	public float lerpTime = .25f;

	public float curTime = 0f;
	public bool crashed = false;
	public bool overheated = false;

	public int frame = 0;

	public GameRunner grScript;

	// Use this for initialization
	void Start () {
		audio.PlayOneShot(startSound);

		PE_Obj bikePEO = this.GetComponent<PE_Obj> ();
		Vector3 temp = bikePEO.transform.position;
		temp.z = -2.25f;
		bikePEO.transform.position = temp;
		//jetFuel = maxFuel;
		flames = GameObject.Find("Flames");

		GameObject scriptParent = GameObject.Find("Main Camera");
		grScript = scriptParent.GetComponent<GameRunner> ();
	}

	void crash() {
		PE_Obj bikePEO = this.GetComponent<PE_Obj> ();

		if (frame % 20 == 0) {
			audio.PlayOneShot (crashSound);
		}
		
		Vector3 tempPos = bikePEO.transform.position;
		Vector3 tempRot = Vector3.zero;
		Vector3 tempVel = Vector3.zero;
		Vector3 tempAcc = Vector3.zero;
		
		//we will need to grab the curtime when it becomes state.crashed (float curTime = Time.time;)
		//crash for 1 to 4 seconds, randomly
		float crashTime = Random.Range (1,4);
		
		if (Time.time < curTime + crashTime) {
			bikePEO.UpdateAccel (tempAcc);
			bikePEO.UpdateVel (tempVel);
			tempPos.z = 2.75f;
			bikePEO.transform.position = tempPos;
			bikePEO.transform.eulerAngles = tempRot;
		}
		else {
			crashed = false;
		}
	}

	void OnGUI () {
		if (overheated) {
			GUI.contentColor = guiColor;
			GUI.Label (new Rect (450, 75, 200, 2000), "Over Heat"); 
		}
	}

	void overheat() {
		PE_Obj bikePEO = this.GetComponent<PE_Obj> ();

		//this has to play only once
		audio.PlayOneShot (overheatSound);
		
		Vector3 tempPos = bikePEO.transform.position;
		Vector3 tempRot = Vector3.zero;
		Vector3 tempVel = Vector3.zero;
		Vector3 tempAcc = Vector3.zero;

		float crashTime = 4f;
		
		if (Time.time < curTime + crashTime) {
			bikePEO.UpdateAccel (tempAcc);
			bikePEO.UpdateVel (tempVel);
			tempPos.z = 2.75f;
			bikePEO.transform.position = tempPos;
			bikePEO.transform.eulerAngles = tempRot;
		}
		else {
			overheated = false;
		}
	}
	
	void Update(){

		////jet management
		if (useJetPack && !overheated) {
			if (Input.GetKey (KeyCode.C) || Input.GetKey (KeyCode.Slash)) {
				jetOn = true;
				flames.renderer.enabled = true;
			} else {
				jetOn = false;
				flames.renderer.enabled = false;
			}
			/*
			if (jetFuel < 0) {
					jetFuel = 0;
					jetOn = false;
			}
			if (jetOn) {
					jetFuel += consumeRate;
			} else
					jetFuel = (jetFuel + regenRate) < maxFuel ? (jetFuel + regenRate) : maxFuel;
			//print (jetFuel);*/
			if (jetOn)
				grScript.temp += jetHeatRate;
		}

		if (Input.GetKey (KeyCode.X) || Input.GetKey (KeyCode.Period)) {
			curAccIn = AccInput.SLOW;
			if (!crashed && frame % 60 == 0) {
				audio.PlayOneShot(slowAccelSound);
			}
		}
		else if (Input.GetKey (KeyCode.Z) || Input.GetKey (KeyCode.Comma)) {
			curAccIn = AccInput.FAST;
			if (!crashed && frame % 60 == 0) {
				audio.PlayOneShot(fastAccelSound);
			}
		}
		else {
			curAccIn = AccInput.NONE;
			if (frame % 60 == 0) {
				audio.PlayOneShot(idle);
			}
		}
		frame++;

		if (Input.GetKey (KeyCode.DownArrow) || Input.GetKey (KeyCode.S)) {
			curDirIn = DirInput.DOWN;
		}
		else if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.W)) {
			curDirIn = DirInput.UP;
		}
		else {
			curDirIn = DirInput.NONE;
		}

		if (Input.GetKey (KeyCode.LeftArrow) || Input.GetKey (KeyCode.A)) {
			curRotIn = RotInput.LEFT;
		}
		else if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.D)) {
			curRotIn = RotInput.RIGHT;
		}
		else {
			curRotIn = RotInput.NONE;
		}
	}

	void FixedUpdate (){
		if (!raceStarted)
			return;

		PE_Obj bikePEO = this.GetComponent<PE_Obj> ();

		//push the bike forward (cheat) if it gets stuck
		//leave in the game as a "god mode"
		if (Input.GetKey (KeyCode.I)) {
			Vector3 tempPos = bikePEO.transform.position;
			tempPos.x += 2;
			bikePEO.transform.position = tempPos;
		}

		float accX = 0f;
		float velZ = 0f;

		switch (curAccIn) {
		case AccInput.FAST:
			accX = fastAcc;
			break;
		case AccInput.SLOW:
			accX = slowAcc;
			break;
		case AccInput.NONE:
			accX = constDecel;
			break;
		}

		switch (curDirIn) {
		case DirInput.DOWN:
			velZ = down;
			break;
		case DirInput.UP:
			velZ = up;
			break;
		case DirInput.NONE:
			velZ = stay;
			break;
		}

		//crashing
		//implement ramp crashes: move it past the ramp to the ground immediately after the ramp
		if (crashed) {
			crash();
		}
		else if (overheated){
			overheat();
		}
		else {
		if (curState == State.ON_GROUND || curState == State.ON_RAMP) {
			bikePEO.UpdateAccel (new Vector3 (accX, 0, 0));
		}

		//automatically slow down if over max speed for any reason (after a jump boost)
		if (bikePEO.vel.x > maxSpeed && curState != State.IN_AIR){
			bikePEO.UpdateAccel(new Vector3(constDecel, 0, 0));
		}
		else if (bikePEO.vel.x > maxSpeed && curState == State.IN_AIR){
				bikePEO.UpdateAccel(new Vector3(airDecel, 0, 0));
		}
		else if (bikePEO.vel.x <= maxAirSpeed && curState == State.IN_AIR){
				bikePEO.UpdateAccel (new Vector3(0,0,0));
		}
		else if (bikePEO.vel.x == maxSpeed && curAccIn != AccInput.NONE && curState != State.IN_AIR)
		{
				bikePEO.UpdateAccel (new Vector3(0,0,0));
		}
	
		if(bikePEO.vel.x <= 0 && curAccIn == AccInput.NONE)
		{
				bikePEO.UpdateAccel (new Vector3(0,0,0));
			bikePEO.UpdateVel (new Vector3(0, bikePEO.vel.y, bikePEO.vel.z));
		}
		
		if (jetOn){
			//this applies jet power directly up
			//bikePEO.UpdateAccel(new Vector3(bikePEO.acc.x, bikePEO.acc.y + jetPower, bikePEO.acc.z));
			//this applies jet power toward bottom of the bike
			bikePEO.acc += transform.up * jetPower;
			curState = State.IN_AIR;
		}

		//lanes
		//this needs to only happen on ground (or full width ramp)
		if (curState == State.ON_GROUND) {
			if(curDirIn == DirInput.UP) {
				bikePEO.UpdateVel (new Vector3 (bikePEO.vel.x, bikePEO.vel.y, velZ));
				prevDirIn = DirInput.UP;
			}
			else if (curDirIn == DirInput.DOWN) {
				bikePEO.UpdateVel (new Vector3 (bikePEO.vel.x, bikePEO.vel.y, velZ));
				prevDirIn = DirInput.DOWN;
			}
			else
			{
				if (bikePEO.transform.position.z > 2.25) {
					//set bike position to go down until it reaches 2.25
					newPosition = new Vector3(bikePEO.transform.position.x, bikePEO.transform.position.y, 2.25f);
					bikePEO.transform.position = Vector3.Lerp(bikePEO.transform.position, newPosition, lerpTime);
					bikePEO.UpdateVel (new Vector3 (bikePEO.vel.x, bikePEO.vel.y, 0f));
					
				}
				else if (bikePEO.transform.position.z <= 2.25 && bikePEO.transform.position.z > .75f) {
					if (prevDirIn == DirInput.UP) {
						//set bike position to go up until it reaches 2.25
						newPosition	= new Vector3(bikePEO.transform.position.x, bikePEO.transform.position.y, 2.25f);
					}
					else if (prevDirIn == DirInput.DOWN) {
						//go down to .75
						newPosition	= new Vector3(bikePEO.transform.position.x, bikePEO.transform.position.y, .75f);
					}
					bikePEO.transform.position = Vector3.Lerp(bikePEO.transform.position, newPosition, lerpTime);
					bikePEO.UpdateVel (new Vector3 (bikePEO.vel.x, bikePEO.vel.y, 0f));
				}
				else if (bikePEO.transform.position.z <= .75f && bikePEO.transform.position.z > -.75f) {
					if (prevDirIn == DirInput.UP) {
						newPosition	= new Vector3(bikePEO.transform.position.x, bikePEO.transform.position.y, .75f);
					}
					else if (prevDirIn == DirInput.DOWN) {
						newPosition	= new Vector3(bikePEO.transform.position.x, bikePEO.transform.position.y, -.75f);
					}
					bikePEO.transform.position = Vector3.Lerp(bikePEO.transform.position, newPosition, lerpTime);
					bikePEO.UpdateVel (new Vector3 (bikePEO.vel.x, bikePEO.vel.y, 0f));
				}
				else if (bikePEO.transform.position.z <= -.75f && bikePEO.transform.position.z > -2.25f) {
					if (prevDirIn == DirInput.UP) {
						newPosition	= new Vector3(bikePEO.transform.position.x, bikePEO.transform.position.y, -.75f);
					}
					else if (prevDirIn == DirInput.DOWN) {
						newPosition	= new Vector3(bikePEO.transform.position.x, bikePEO.transform.position.y, -2.25f);
					}
					bikePEO.transform.position = Vector3.Lerp(bikePEO.transform.position, newPosition, lerpTime);
					bikePEO.UpdateVel (new Vector3 (bikePEO.vel.x, bikePEO.vel.y, 0f));
				}
				else {
					newPosition = new Vector3(bikePEO.transform.position.x, bikePEO.transform.position.y, -2.25f);
					bikePEO.transform.position = Vector3.Lerp(bikePEO.transform.position, newPosition, lerpTime);
					bikePEO.UpdateVel (new Vector3 (bikePEO.vel.x, bikePEO.vel.y, 0f));

				}
			}
		}

		//wheelie
		if (curState == State.ON_GROUND) {
			if (curRotIn == RotInput.LEFT && bikePEO.transform.eulerAngles.z <= maxAngle + 25 && (Input.GetKey (KeyCode.X) || Input.GetKey (KeyCode.Z) || Input.GetKey (KeyCode.Period) || Input.GetKey (KeyCode.Comma))){
				bikePEO.transform.Rotate(Vector3.forward * (rotSpeed * Time.deltaTime));
			}
			else if ((bikePEO.transform.eulerAngles.z >= maxAngle + 25 && bikePEO.transform.eulerAngles.z <= maxAngle + 25 + 4) || (bikePEO.transform.eulerAngles.z <= 360 - maxAngle && bikePEO.transform.eulerAngles.z >= 360 - maxAngle - 5))
			{
				//print (bikePEO.transform.eulerAngles.z);
				crashed = true;
				curTime = Time.time;
			} 
			else if (bikePEO.transform.eulerAngles.z >= maxAngle + 25 + 5)
			{
				bikePEO.transform.Rotate(Vector3.forward * (rotSpeed * Time.deltaTime));
			}
			else if (bikePEO.transform.eulerAngles.z >= 0) {
				bikePEO.transform.Rotate(Vector3.back * (rotSpeed * Time.deltaTime));
			}

		}

		//in air rotation
		if (curState == State.IN_AIR) {
			if (bikePEO.transform.eulerAngles.z >= maxAngle - 1 && bikePEO.transform.eulerAngles.z < 180) {
				Vector3 temp = bikePEO.transform.eulerAngles;
				temp.z = maxAngle - 1;
				bikePEO.transform.eulerAngles = temp;
			}
			
			if (bikePEO.transform.eulerAngles.z <= 360 - maxAngle && bikePEO.transform.eulerAngles.z > 180) {
				Vector3 temp = bikePEO.transform.eulerAngles;
				temp.z = 360 - maxAngle + 1;
				bikePEO.transform.eulerAngles = temp;
			}

			if (bikePEO.transform.eulerAngles.z >= 360-maxAngle || bikePEO.transform.eulerAngles.z <= maxAngle - 1) {
				if (curRotIn == RotInput.RIGHT) {
					bikePEO.transform.Rotate(Vector3.back * (rotSpeed * Time.deltaTime));
				}
				else if (curRotIn == RotInput.LEFT) {
					bikePEO.transform.Rotate(Vector3.forward * (rotSpeed * Time.deltaTime));
				}
			}
		}

		if (curState == State.ON_RAMP) {
			if ((bikePEO.transform.eulerAngles.z >= maxAngle && bikePEO.transform.eulerAngles.z <= maxAngle + 4) || (bikePEO.transform.eulerAngles.z <= 360 - maxAngle && bikePEO.transform.eulerAngles.z >= 360 - maxAngle - 5))
			{
				Vector3 tempPos = bikePEO.transform.position;
				tempPos.x += 2;
				tempPos.y += 3;
				bikePEO.transform.position = tempPos;
				crashed = true;
				curTime = Time.time;
			} 
			
			//rotation set to 0
			Vector3 tempRot = Vector3.zero;
			bikePEO.transform.eulerAngles = tempRot;
		}
		
		}

	}
}
