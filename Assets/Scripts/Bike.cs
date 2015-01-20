using UnityEngine;
using System.Collections;

//test

public class Bike : MonoBehaviour {
	//public PE_Obj bikePeo = this.GetComponent<PE_Obj> ();

	public enum State {
		IN_AIR,
		ON_GROUND,
		ON_RAMP,
		CRASHING,
		CRASHED,
	}

	enum AccInput {
		NONE,
		SLOW,
		FAST
	}

	enum DirInput {
		UP,
		DOWN,
		NONE
	}

	AccInput curAccIn = AccInput.NONE;
	DirInput curDirIn = DirInput.NONE;


	public State curState = State.ON_GROUND;

	//max speed is same regardless of fast or slow accel
	float maxSpeed = 8f;
	float maxAngle = 45f;


	float rotSpeed = 10f;
	float slowAcc = 3.25f;
	float fastAcc = 5f;
	float constDecel = -8f;

	float up =5f;
	float down = -5f;
	float stay = 0f;

	// Use this for initialization
	void Start () {
		
	}

	void Update(){
		//should return -1 or 1, gives us direction based on wasd or arrow key input 
		//if -1 rotate left, if 1 rotate right, if 0 rotate towards parallel w/ surface
		//float rotateInput = Input.GetAxis("Horizontal"); 
		
		if (Input.GetKey (KeyCode.X)) {
			curAccIn = AccInput.SLOW;
		}
		else if (Input.GetKey (KeyCode.Z)) {
			curAccIn = AccInput.FAST;
		}
		else {
			curAccIn = AccInput.NONE;
		}

		if (Input.GetKey (KeyCode.DownArrow)) {
			curDirIn = DirInput.DOWN;
		}
		else if (Input.GetKey (KeyCode.UpArrow)) {
			curDirIn = DirInput.UP;
		}
		else {
			curDirIn = DirInput.NONE;
		}



	}

	void FixedUpdate (){
		PE_Obj bikePEO = this.GetComponent<PE_Obj> ();

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

		if (curState == State.ON_GROUND || curState == State.ON_RAMP) {
			bikePEO.UpdateAccel (new Vector3(accX, 0, 0));

			if (bikePEO.vel.x >= maxSpeed && curAccIn != AccInput.NONE)
			{
				//print ("velfastcheck");
				bikePEO.UpdateAccel (new Vector3(0,0,0));
			}

			if(bikePEO.vel.x <= 0 && curAccIn == AccInput.NONE)
			{
				//print("velstopcheck");
				bikePEO.UpdateAccel (new Vector3(0,0,0));
				bikePEO.UpdateVel (new Vector3(0, bikePEO.vel.y, bikePEO.vel.z));
			}

			//need to make this move a certain distance on input, then stop at that distance if key is released or keep going until
			//	hit wall otherwise
			bikePEO.UpdateVel (new Vector3 (bikePEO.vel.x, bikePEO.vel.y, velZ));

		}
		//TODO: call update temp based on input

	}
}
