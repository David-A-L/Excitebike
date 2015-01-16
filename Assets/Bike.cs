using UnityEngine;
using System.Collections;

public class Bike : MonoBehaviour {

	public enum State {
		IN_AIR,
		ON_GROUND,
		CRASHING,
		CRASHED,
	}

	enum AccInput {
		NONE,
		SLOW,
		FAST
	}

	AccInput curAccIn = AccInput.NONE;
	public State curState = State.IN_AIR;

	//these limits may have to co in the peo
	float maxSpeed = 10f;
	float maxAngle = 45f;


	float rotSpeed = 10f;
	float slowAcc = 1f;
	float fastAcc = 1.5f;
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

	}

	void FixedUpdate (){
		PE_Obj bikePEO = this.GetComponent<PE_Obj> ();

		float accX = 0f;

		switch (curAccIn) {
		case AccInput.FAST:
			accX = fastAcc;
			break;
		case AccInput.SLOW:
			accX = slowAcc;
			break;
		case AccInput.NONE:
			break;
		}

		if (curState == State.ON_GROUND) {
			bikePEO.UpdateAccel ( new Vector3(accX, 0, 0));
		}
		//TODO: call update temp based on input

	}
}
