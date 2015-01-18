﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PE_GravType {
	none,
	constant,
	planetary
}

public enum PE_Collider {
	sphere,
	aabb,
	plane
}

public class PhysEngine : MonoBehaviour {
	static public List<PE_Obj>	objs;
	
	public Vector3		gravity = new Vector3(0,-9.8f,0);
	
	// Use this for initialization
	void Awake() {
		objs = new List<PE_Obj>();
	}
	
	
	void FixedUpdate () {
		// Handle the timestep for each object
		float dt = Time.fixedDeltaTime;
		foreach (PE_Obj po in objs) {
			TimeStep(po, dt);
		}
		
		// Resolve collisions
		
		
		// Finalize positions
		foreach (PE_Obj po in objs) {
			po.transform.position = po.pos1;
		}
		
	}
	
	
	public void TimeStep(PE_Obj po, float dt) {
		if (po.still) {
			po.pos0 = po.pos1 = po.transform.position;
			return;
		}
		
		// Velocity
		po.vel0 = po.vel;
		Vector3 tAcc = po.acc;
		switch (po.grav) {
		case PE_GravType.constant:
			tAcc += gravity;
			break;
		}
		po.vel += tAcc * dt;
		
		// Position
		po.pos1 = po.pos0 = po.transform.position;
		po.pos1 += po.vel * dt;
		
	}
}
