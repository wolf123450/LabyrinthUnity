using UnityEngine;
using System.Collections;
using System;

public class RobotAI : MonoBehaviour {
	private enum State { SLEEPING, ALERT, CHARGING, IDLE}
	private State state;
	private Vector3 destination;
	private float angularSpeed_fast;
	private float speed_fast;
	private float angularSpeed_slow;
	private float speed_slow;
	private bool playerSighted;
	private int waitCount;
	private bool firstTime;
	
	
	void Start () {
		
		angularSpeed_fast = 120f;
		speed_fast = 6f;
		angularSpeed_slow = 100f;
		speed_slow = 3f;
		
		state = State.SLEEPING;
		playerSighted = false;
		firstTime = true;
	}
	
	void Update () {
		switch (state) {
		case State.SLEEPING: Sleeping(); break;
		case State.ALERT: Alert(); break;
		case State.CHARGING: Charging(); break;
		case State.IDLE: Idle(); break;
		}
	}


	
	void Sleeping () {


	}
	
	void Alert () {
		NavMeshAgent agent = GetComponentInParent<NavMeshAgent> ();
		state = State.ALERT;
		if (!agent.destination.Equals (destination)) {
			agent.angularSpeed = angularSpeed_slow;
			agent.speed = speed_slow;
			agent.SetDestination (destination);
			Debug.Log ("Alert");
		} 
		if (isClose (GetComponentInParent<Transform> ().position, destination)) {
			state = State.IDLE;
		}

		if (playerSighted) {
			state = State.CHARGING;
			Debug.Log("GOING TO CHARGE!");
		}
		

	}
	
	void Wandering () {
		//randomly move to a few interesctions near destination
		
		//if (player found) { state = State.CHARGING; }
		//else if (wander time equals 0 && player not found) { state = State.RETURNING; }
	}
	
	void Charging () {
		Debug.Log("CHARGING!");
		if (!playerSighted) { 

		} else {
			NavMeshAgent agent = GetComponentInParent<NavMeshAgent> ();
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			Vector3 playerLocation = player.GetComponent<Transform>().position;
			
			if (!agent.destination.Equals (playerLocation)) {
				agent.angularSpeed = angularSpeed_fast;
				agent.speed = speed_fast;
				agent.destination = playerLocation;
				Debug.Log ("Charging");
			}
			playerSighted = false;
		}
	}
	
	void Idle () {
		NavMeshAgent agent = GetComponentInParent<NavMeshAgent> ();
		Debug.Log ("IDLE");
		if(firstTime) {
			waitCount = 500;
			firstTime = false;
		}
		else if(waitCount > 0){
			waitCount--;
		}
		else
		{
			state = State.SLEEPING;
			firstTime = true;
			
		}
			


		if (playerSighted) {
			state = State.CHARGING;
			Debug.Log("GOING TO CHARGE!");
		}
	}
	
	public void Notify(Vector3 location)
	{
		Debug.Log ("ROBOT NOTIFIED!");
		destination = location;
		state = State.ALERT;


	}

	
	bool isClose(Vector3 first, Vector3 second)
	{
		if (Math.Abs(first.x - second.x) <= 1) 
		{
			if(Math.Abs(first.z - second.z) <=1)
			{
				return true;
			}
		}
		return false;
		
		
	}
}
