using UnityEngine;
using System.Collections;

//test

public class Bike : MonoBehaviour {

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

	public AccInput curAccIn = AccInput.NONE;
	public DirInput curDirIn = DirInput.NONE;
	public DirInput prevDirIn = DirInput.NONE;
	public RotInput curRotIn = RotInput.NONE;
	public State curState = State.ON_GROUND;

	//max speed is same regardless of fast or slow accel
	float maxSpeed = 8f;
	float maxAirSpeed = 8f;
	float maxAngle = 45f;

	public Vector3 newPosition = Vector3.zero;
	//public float transitionTime = 1f;

	float rotSpeed = 70f;
	public float slowAcc = 3.25f;
	public float fastAcc = 5f;
	float constDecel = -8f;
	float airDecel = -4f;

	float up = 5f;
	float down = -5f;
	float stay = 0f;

	public float curTime = 0f;
	public bool crashed = false;

	// Use this for initialization
	void Start () {
		PE_Obj bikePEO = this.GetComponent<PE_Obj> ();
		Vector3 temp = bikePEO.transform.position;
		temp.z = -2.25f;
		bikePEO.transform.position = temp;
	}

	void crash() {
		PE_Obj bikePEO = this.GetComponent<PE_Obj> ();

		print ("crashed!");
		
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

		if (Input.GetKey (KeyCode.LeftArrow)) {
			curRotIn = RotInput.LEFT;
		}
		else if (Input.GetKey (KeyCode.RightArrow)) {
			curRotIn = RotInput.RIGHT;
		}
		else {
			curRotIn = RotInput.NONE;
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

		//crashing
		//implement ramp crashes: move it past the ramp to the ground immediately after the ramp
		if (crashed) {
			crash();
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
			//print ("velfastcheck");
			bikePEO.UpdateAccel (new Vector3(0,0,0));
		}
	
		if(bikePEO.vel.x <= 0 && curAccIn == AccInput.NONE)
		{
			//print("velstopcheck");
			bikePEO.UpdateAccel (new Vector3(0,0,0));
			bikePEO.UpdateVel (new Vector3(0, bikePEO.vel.y, bikePEO.vel.z));
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
					bikePEO.transform.position = Vector3.Lerp(bikePEO.transform.position, newPosition, .35f);
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
					bikePEO.transform.position = Vector3.Lerp(bikePEO.transform.position, newPosition, .35f);
					bikePEO.UpdateVel (new Vector3 (bikePEO.vel.x, bikePEO.vel.y, 0f));
				}
				else if (bikePEO.transform.position.z <= .75f && bikePEO.transform.position.z > -.75f) {
					if (prevDirIn == DirInput.UP) {
						newPosition	= new Vector3(bikePEO.transform.position.x, bikePEO.transform.position.y, .75f);
					}
					else if (prevDirIn == DirInput.DOWN) {
						newPosition	= new Vector3(bikePEO.transform.position.x, bikePEO.transform.position.y, -.75f);
					}
					bikePEO.transform.position = Vector3.Lerp(bikePEO.transform.position, newPosition, .35f);
					bikePEO.UpdateVel (new Vector3 (bikePEO.vel.x, bikePEO.vel.y, 0f));
				}
				else if (bikePEO.transform.position.z <= -.75f && bikePEO.transform.position.z > -2.25f) {
					if (prevDirIn == DirInput.UP) {
						newPosition	= new Vector3(bikePEO.transform.position.x, bikePEO.transform.position.y, -.75f);
					}
					else if (prevDirIn == DirInput.DOWN) {
						newPosition	= new Vector3(bikePEO.transform.position.x, bikePEO.transform.position.y, -2.25f);
					}
					bikePEO.transform.position = Vector3.Lerp(bikePEO.transform.position, newPosition, .35f);
					bikePEO.UpdateVel (new Vector3 (bikePEO.vel.x, bikePEO.vel.y, 0f));
				}
				else {
					newPosition = new Vector3(bikePEO.transform.position.x, bikePEO.transform.position.y, -2.25f);
					bikePEO.transform.position = Vector3.Lerp(bikePEO.transform.position, newPosition, .35f);
					bikePEO.UpdateVel (new Vector3 (bikePEO.vel.x, bikePEO.vel.y, 0f));

				}
			}
		}

		//wheelie
		if (curState == State.ON_GROUND) {

			//at a certain angle we need to crash, but for now we just max out
			if (curRotIn == RotInput.LEFT && bikePEO.transform.eulerAngles.z <= maxAngle && (Input.GetKey (KeyCode.X) || Input.GetKey (KeyCode.Z))){
				bikePEO.transform.Rotate(Vector3.forward * (rotSpeed * Time.deltaTime));
			}
			else if ((bikePEO.transform.eulerAngles.z >= maxAngle && bikePEO.transform.eulerAngles.z <= maxAngle + 4) || (bikePEO.transform.eulerAngles.z <= 360 - maxAngle && bikePEO.transform.eulerAngles.z >= 360 - maxAngle - 5))
			{
				crashed = true;
				curTime = Time.time;
			} 
			else if (bikePEO.transform.eulerAngles.z >= maxAngle + 5)
			{
				/*Vector3 temp = bikePEO.transform.eulerAngles;
				temp.z = 0;
				bikePEO.transform.eulerAngles = temp;*/
				bikePEO.transform.Rotate(Vector3.forward * (rotSpeed * Time.deltaTime));
			}
			else if (bikePEO.transform.eulerAngles.z >= 0) {
				bikePEO.transform.Rotate(Vector3.back * (rotSpeed * Time.deltaTime));
			}

		}

		//minor bug: input right should override input left, not hugely important
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

				
			
		//TODO: call update temp based on input
		}

	}
}
