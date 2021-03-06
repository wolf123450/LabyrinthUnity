﻿using UnityEngine;
using System.Collections;
using System;

public class CameraAI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnTriggerEnter(Collider obj) {
		if (obj.tag.Equals ("Player")) {
			Vector3 location = GetComponentInParent<Transform>().position;
			GameObject[] robots = GameObject.FindGameObjectsWithTag("Robot");
			double shortestDistance = double.PositiveInfinity;
			int nearestRobotIndex = 0;
			for(int i = 0; i < robots.Length; i++)
			{
				float distance = Vector3.Distance(robots[i].GetComponent<Transform>().position, location);
				if( distance < shortestDistance)
				{
					shortestDistance = distance;
					nearestRobotIndex = i;
				}
			}
			robots[nearestRobotIndex].GetComponentInChildren<RobotAI>().Notify(location);
		}
	}
	
}
