using UnityEngine;
using System.Collections;

public class PE_Obj : MonoBehaviour {

	public bool			still = false;
	public PE_Collider	coll = PE_Collider.sphere;
	//public PE_Collider	coll2 = PE_Collider.aabb;
	public PE_GravType	grav = PE_GravType.constant;
	
	public Vector3		acc = Vector3.zero;
	
	public Vector3		vel = Vector3.zero;
	public Vector3		vel0 = Vector3.zero;
	
	public Vector3		pos0 = Vector3.zero;
	public Vector3		pos1 = Vector3.zero;

	//public PE_Dir		dir = PE_Dir.still;
	
	//public PE_Obj		ground = null; // Stores whether this is on the ground
	
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
	
	
	void OnTriggerEnter(Collider other) {
		// Ignore collisions of still objects
		if (still) return;
		
		PE_Obj otherPEO = other.GetComponent<PE_Obj>();
		if (otherPEO == null) return;
		
		ResolveCollisionWith(otherPEO);
	}
	
	void OnTriggerStay(Collider other) {
		OnTriggerEnter(other);
	}

	/*void OnTriggerExit(Collider other) {
		// Ignore collisions of still objects
		if (still) return;
		
		PE_Obj otherPEO = other.GetComponent<PE_Obj>();
		if (otherPEO == null) return;
		
		// This sets ground to null if we fall off of the current ground
		// Jumping will also set ground to null
		//if (ground == otherPEO) {
		//	ground = null;
		//}
	}*/
	
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
			//Bike thisBike = this.GetComponent<Bike>();
			switch (that.coll) {
			case PE_Collider.aabb:

				thisBike.curState = Bike.State.ON_GROUND;

				// In Progress (we might need to add top left once we get the bike rotating?

				// AABB / AABB collision
				float eX1, eY1, eX2, eY2, dX, dY, eX0, eY0;

				Vector3 overlap = Vector3.zero;
				Vector3 thatP = that.transform.position;
				Vector3 delta = pos1 - thatP;
				//if (delta.x >= 0 && delta.y >= 0) 
				//{ 
					// Top, Right (of that)
					// Get the edges that we're concerned with
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
						vel.y = 0;
					}
				//} 
				/*else if (delta.x >= 0 && delta.y < 0) 
				{ 
					// Bottom, Right

				} 
				else if (delta.x < 0 && delta.y < 0) 
				{ 
					// Bottom, Left

				} 
				else if (delta.x < 0 && delta.y >= 0) { 
					// Top, Left

				}*/

				break;

		/*case PE_Collider.aabb:

			switch (that.coll) {
			case PE_Collider.aabb:

				//we need to be careful with this: top of ramp will have same collision... also we need to change this state when the bike isn't colliding with anything
				thisBike.curState = Bike.State.ON_GROUND;

				// AABB / AABB collision
				// Axis-Aligned Bounding Box
				// With AABB collisions, we're usually concerned with corners and deciding which corner to consider when making comparisons.
				// I believe that this corner should be determined by looking at the velocity of the moving body (this one)
				
				Vector3 a0, a1, b; // a0-moving corner last frame, a1-moving corner now, b-comparison corner on other object
				a0 = a1 = b = Vector3.zero;	 // Sets a default value to keep the compiler from complaining
				Vector3 delta = pos1 - pos0;
				
				if (dir == PE_Dir.down) {
					// Just resolve to be on top
					a1 = pos1;
					a1.y -= transform.lossyScale.y/2f;
					b = that.pos1;
					b.y += that.transform.lossyScale.y/2f;
					if (b.y > a1.y) {
						posFinal.y += Mathf.Abs( a1.y - b.y );
					}
					// Handle vel
					vel.y = 0;
					
					//if (ground == null) ground = that;
					break; // Exit this switch statement: switch (that.coll)
				}
				
				if (dir == PE_Dir.up) {
					// Just resolve to be below
					a1 = pos1;
					a1.y += transform.lossyScale.y/2f;
					b = that.pos1;
					b.y -= that.transform.lossyScale.y/2f;
					if (b.y < a1.y) {
						posFinal.y -= Mathf.Abs( a1.y - b.y );
					}
					// Handle vel
					vel.y = 0;
					
					break; // Exit this switch statement: switch (that.coll)
				}
				
				if (dir == PE_Dir.upRight) { // Bottom, Left is the comparison corner
					a1 = pos1;
					a1.x += transform.lossyScale.x/2f;
					a1.y += transform.lossyScale.y/2f;
					a0 = a1 - delta;
					b = that.pos1;
					b.x -= that.transform.lossyScale.x/2f;
					b.y -= that.transform.localScale.y/2f;
				}
				
				if (dir == PE_Dir.upLeft) { // Bottom, Right is the comparison corner
					a1 = pos1;
					a1.x -= transform.lossyScale.x/2f;
					a1.y += transform.lossyScale.y/2f;
					a0 = a1 - delta;
					b = that.pos1;
					b.x += that.transform.lossyScale.x/2f;
					b.y -= that.transform.localScale.y/2f;
				}
				
				if (dir == PE_Dir.downLeft) { // Top, Right is the comparison corner
					a1 = pos1;
					a1.x -= transform.lossyScale.x/2f;
					a1.y -= transform.lossyScale.y/2f;
					a0 = a1 - delta;
					b = that.pos1;
					b.x += that.transform.lossyScale.x/2f;
					b.y += that.transform.localScale.y/2f;
				}
				
				if (dir == PE_Dir.downRight) { // Top, Left is the comparison corner
					a1 = pos1;
					a1.x += transform.lossyScale.x/2f;
					a1.y -= transform.lossyScale.y/2f;
					a0 = a1 - delta;
					b = that.pos1;
					b.x -= that.transform.lossyScale.x/2f;
					b.y += that.transform.localScale.y/2f;
				}
				
				// In the x dimension, find how far along the line segment between a0 and a1 we need to go to encounter b
				float u = (b.x - a0.x) / (a1.x - a0.x);
				
				// Determine this point using linear interpolation (see the appendix of the book)
				Vector3 pU = (1-u)*a0 + u*a1;
				
				// Use pU.y vs. b.y to tell which side of PE_Obj "that" PE_Obj "this" should be on
				switch (dir) {
				case PE_Dir.upRight:
					if (pU.y > b.y) { // hit the left side
						posFinal.x -= Mathf.Abs(a1.x - b.x);
						
						// Handle vel
						vel.x = 0;
						
					} else { // hit the bottom
						posFinal.y -= Mathf.Abs(a1.y - b.y);
						
						// Handle vel
						vel.y = 0;
						
					}
					break;
					
				case PE_Dir.downRight:
					if (pU.y < b.y) { // hit the left side
						posFinal.x -= Mathf.Abs(a1.x - b.x);
						
						// Handle vel
						vel.x = 0;
						
					} else { // hit the top
						posFinal.y += Mathf.Abs(a1.y - b.y);
						
						// Handle vel
						vel.y = 0;
						
						//if (ground == null) ground = that;
					}
					break;
					
				case PE_Dir.upLeft:
					if (pU.y > b.y) { // hit the right side
						posFinal.x += Mathf.Abs(a1.x - b.x);
						
						// Handle vel
						vel.x = 0;
						
					} else { // hit the bottom
						posFinal.y -= Mathf.Abs(a1.y - b.y);
						
						// Handle vel
						vel.y = 0;
						
					}
					break;
					
				case PE_Dir.downLeft:
					if (pU.y < b.y) { // hit the right side
						posFinal.x += Mathf.Abs(a1.x - b.x);
						
						// Handle vel
						vel.x = 0;
						
					} else { // hit the top
						posFinal.y += Mathf.Abs(b.y - a1.y);
						
						// Handle vel
						vel.y = 0;
						
						//if (ground == null) ground = that;
					}
					break;
				}
				
				break;*/
			

			case PE_Collider.plane:
				thisBike.curState = Bike.State.ON_RAMP;
				GameObject rampGO = that.gameObject;

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
				//vel = vel*Mathf.Cos(Quaternion.Angle(that.gameObject.transform.rotation,this.transform.rotation));
				vel = Vector3.Project(vel, rampVec);
				acc = Vector3.Project(acc, rampVec);

				//RayCasting from corner toward plane to get depth of penetration
				Ray cornerRay = new Ray(cornerPos,rampGO.transform.up);
				RaycastHit hit = new RaycastHit();
				if(!collider.Raycast(cornerRay,out hit,10f)){
					print ("Didn't hit anything");
					return;
				}
				Vector3 temp = transform.position;
				temp += rampGO.transform.up * hit.distance;
				transform.position = temp;
				break;

			}

			break;
		}

		//transform.position = pos1 = posFinal;
	

		//fixes: needs to rotate smoothly, needs to be overrided if there is rotation input(and the bike
		//	is moving at a fast enough x velocity or is in air)
		//this.transform.rotation = that.gameObject.transform.rotation;
	}//collision resolution
	
	
}
