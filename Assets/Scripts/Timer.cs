﻿using UnityEngine;
using System.Collections;

//add something that makes this pop up with "LAP: [then beneath the time as we jump over the lap ramp]"

public class Timer : MonoBehaviour {
	private float startTime;
	public string textTime;
	bool numLaps = 0;

	void Awake () {
		startTime = Time.time;
	}

	void OnGUI () {
		float guiTime = Time.time - startTime;

		int minutes = (int)guiTime / 60;
		int seconds = (int)guiTime % 60;
		int fracSec = (int)(guiTime * 100) % 100;

		textTime = string.Format ("{0:00}:{1:00}:{2:00}", minutes, seconds, fracSec);
		GUI.Label (new Rect (800, 550, 100, 1000), textTime); 
	}

	//display lap time after lap and finish
	void OnTriggerEnter() {


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
