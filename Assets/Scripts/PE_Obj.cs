using UnityEngine;
using System.Collections;

public class PE_Obj : MonoBehaviour {

	public bool			still = false;
	public PE_Collider	coll = PE_Collider.sphere;
	public PE_GravType	grav = PE_GravType.constant;
	
	public Vector3		acc = Vector3.zero;
	
	public Vector3		vel = Vector3.zero;
	public Vector3		vel0 = Vector3.zero;
	
	public Vector3		pos0 = Vector3.zero;
	public Vector3		pos1 = Vector3.zero;

	public float guiTime;
	public string textTime;
	public bool lapDisplay = false;
	public float landingEase = 35f;
	public float bouncePower = .5f;
	public float bounceSlow = .5f;
	public float minBounceVel = 10f;
	public float bounceFloorVel = 9f;

	Vector3 lastRampAngle = Vector3.zero;
	
	void Start() {
		if (PhysEngine.objs.IndexOf(this) == -1) {
			PhysEngine.objs.Add(this);
		}
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void UpdateAccel (Vector3 accIn){
		acc = accIn;
	}

	public void UpdateVel (Vector3 velIn){
		vel = velIn;
	}

	void OnGUI ()
	{
		if (lapDisplay == true) {
			GUI.Label (new Rect (450, 100, 100, 1000), textTime);
		}
	}
	
	void lapTime () {
		
		int minutes = (int)guiTime / 60;
		int seconds = (int)guiTime % 60;
		int fracSec = (int)(guiTime * 100) % 100;
		
		textTime = string.Format ("{0:00}:{1:00}:{2:00}", minutes, seconds, fracSec);
		//GUI.Label (new Rect (300, 250, 100, 1000), textTime); 
	}
	
	void OnTriggerEnter(Collider other) {
		//flash lap time in center of screen
		if (this.tag == "Bike" && other.tag == "LapRamp") {
			guiTime = Time.time - 2.5f;
			lapTime();
			lapDisplay = true;
		}
		if (Time.time > guiTime + 4f + 2.5f) {
			lapDisplay = false;
		}

		// Ignore collisions of still objects
		if (still) return;
		
		PE_Obj otherPEO = other.GetComponent<PE_Obj>();
		if (otherPEO == null) return;
		
		ResolveCollisionWith(otherPEO);
	}
	
	void OnTriggerStay(Collider other) {
		OnTriggerEnter(other);
	}

	void ResolveCollisionWith(PE_Obj that) {
		// Assumes that "that" is still
		Vector3 posFinal = pos1; // Sets a defaut value for posFinal
		Bike thisBike = this.GetComponent<Bike>();
		
		switch (this.coll) {
		case PE_Collider.sphere:
			
			switch (that.coll) {
			case PE_Collider.sphere:
				// Sphere / Sphere collision
				float thisR, thatR, rad;
				// Note, this doesn't work with non-uniform or negative scales!
				thisR = Mathf.Max( this.transform.lossyScale.x, this.transform.lossyScale.y, this.transform.lossyScale.z ) / 2;
				thatR = Mathf.Max( that.transform.lossyScale.x, that.transform.lossyScale.y, that.transform.lossyScale.z ) / 2;
				rad = thisR + thatR;
				
				Vector3 delta = pos1 - that.transform.position;
				delta.Normalize();
				delta *= rad;
				
				posFinal = that.transform.position + delta;
				break;
			}
			
			break;
			
		case PE_Collider.aabb:
			bool bounce = false;
			float bounceVel = -vel.y * bouncePower;
			bounceVel = bounceVel > bounceFloorVel? bounceVel:bounceFloorVel;
			if (thisBike.curState == Bike.State.IN_AIR && vel.y < 0f && Mathf.Abs(vel.y) > minBounceVel){
				float myAngle = transform.eulerAngles.z;
				if (myAngle > 180f)
					myAngle = 360f - myAngle; 
				float angleDiff = Mathf.Abs( that.transform.eulerAngles.z - myAngle);
				if (angleDiff > landingEase)
					bounce = true;
			}
			switch (that.coll) {
			case PE_Collider.aabb:

				thisBike.curState = Bike.State.ON_GROUND;

				// AABB / AABB collision
				float eX1, eY1, eX2, eY2, dX, dY, eX0, eY0;

				Vector3 overlap = Vector3.zero;
				Vector3 thatP = that.transform.position;
				Vector3 delta = pos1 - thatP;

				eX0 = pos0.x - this.transform.lossyScale.x / 2;
				eY0 = pos0.y - this.transform.lossyScale.y / 2;
				eX1 = pos1.x - this.transform.lossyScale.x / 2;
				eY1 = pos1.y - this.transform.lossyScale.y / 2;
				eX2 = thatP.x + that.transform.lossyScale.x / 2 ;
				eY2 = thatP.y + that.transform.lossyScale.y / 2 ;


				// Distance traveled this step
				dX = eX1 - eX0;
				dY = eY1 - eY0;
				float uX = 1;
				float uY = 1;
				if (eX1 < eX2) 
				{ 
					// Overlap in X direction
					uX = 1 - (eX2-eX1) / (eX0-eX1);
				}
				if (eY1 < eY2) 
				{
					// Overlap in Y direction
					uY = 1 - (eY2-eY1) / (eY0-eY1);
				}
				// Find Overlaps (positive is an overlap, negative 
				//overlap.x = uX;
				overlap.y = uY;

				if (overlap.y >= 0)
				{
					//print ("asdf");
					Vector3 moved = transform.position;
					moved.y += (eY2 - eY1);
					transform.position = moved;
					if (vel.y < 0)
						vel.y = 0;
				}

				break;
			

			case PE_Collider.plane:
				thisBike.curState = Bike.State.ON_RAMP;

				GameObject rampGO = that.gameObject;
				if (rampGO.tag == "Ramp" || rampGO.tag == "LapRamp")
					grav = PE_GravType.none;

				Vector3 cornerPos = this.gameObject.transform.position;
				//check which corner has collided
				if (rampGO.transform.rotation.z > 0){
					cornerPos.x += this.transform.lossyScale.x/2;
				}
				else {
					cornerPos.x -= this.transform.lossyScale.x/2;
				}
				cornerPos.y -= this.transform.lossyScale.y/2;
				Vector3 rampVec = rampGO.transform.right;
				vel = rampVec * vel.magnitude;
				//acc = rampVec * acc.magnitude;

				//RayCasting from corner toward plane to get depth of penetration
				Ray cornerRay = new Ray(cornerPos,rampGO.transform.up);
				Debug.DrawRay(cornerPos, rampGO.transform.up * 100f);
				RaycastHit hit = new RaycastHit();
				if(!Physics.Raycast(cornerRay,out hit,100f)){
					return;
				}
				/*float dist = Mathf.Sqrt(transform.lossyScale.x*transform.lossyScale.x + transform.lossyScale.y*transform.lossyScale.y);
				dist -= hit.distance;*/
				Vector3 temp = transform.position;
				temp += rampGO.transform.up * (hit.distance);
				transform.position = temp;
				lastRampAngle = rampGO.transform.eulerAngles;
				break;

			}
			if (bounce){
				//print ("bouncing");
				vel.y = bounceVel;
				vel.x -= vel.x * bounceSlow;
				thisBike.curState = Bike.State.IN_AIR;
			}
			break;
		}
	}

	//collision resolution

	void OnTriggerExit(Collider other){
		if (other.tag == "RampBoost") {
			Bike bScript = this.gameObject.GetComponent<Bike>();
			bScript.curState = Bike.State.IN_AIR;
			this.transform.eulerAngles = lastRampAngle;
		}
		grav = PE_GravType.constant;
	}
	
}
