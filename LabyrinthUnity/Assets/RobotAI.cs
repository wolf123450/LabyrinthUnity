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
	private AudioSource movingSound;



	
	void Start () {
		
		angularSpeed_fast = 120f;
		speed_fast = 6f;
		angularSpeed_slow = 100f;
		speed_slow = 3f;
		
		state = State.SLEEPING;
		playerSighted = false;
		firstTime = true;
		movingSound = GetComponentInParent<AudioSource> ();
		if (movingSound.isPlaying) {
			movingSound.Stop();
		}
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
		//Debug.Log ("ROBOT SLEEPING");
		if (movingSound.isPlaying) {
			movingSound.Stop();
		}


	}
	
	void Alert () {
		NavMeshAgent agent = GetComponentInParent<NavMeshAgent> ();
		state = State.ALERT;

		if (!movingSound.isPlaying) {
			movingSound.Play();
		}

		if (!agent.destination.Equals (destination)) {
			agent.angularSpeed = angularSpeed_slow;
			agent.speed = speed_slow;
			agent.SetDestination (destination);
			//Debug.Log (" ROBOT Alert");
		} 
		if (isClose (GetComponentInParent<Transform> ().position, destination)) {
			state = State.IDLE;
		}

		if (canSeePlayer()) {
			state = State.CHARGING;
			agent.SetDestination(GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ().position);
			Debug.Log("ROBOT GOING TO CHARGE!");
		}
		

	}
	
	void Wandering () {
		//randomly move to a few interesctions near destination
		
		//if (player found) { state = State.CHARGING; }
		//else if (wander time equals 0 && player not found) { state = State.RETURNING; }
	}
	
	void Charging () {
		NavMeshAgent agent = GetComponentInParent<NavMeshAgent> ();
		Vector3 playerPosition = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ().position;
		if (!movingSound.isPlaying) {
			movingSound.Play();
		}

		if (canSeePlayer()) { 
			agent.SetDestination(playerPosition);

		} 
		else {
			if(firstTime) {
				Debug.Log("STILL CHASING BUT CAN'T SEE");
				waitCount = 10;
				agent.SetDestination(playerPosition);
				firstTime = false;
			}
			else if(waitCount > 0){
				Debug.Log("STILL CHASING BUT CAN'T SEE");
				waitCount--;
				agent.SetDestination(playerPosition);
			}
			else
			{
				state = State.IDLE;
				firstTime = true;
				
			}
;
		}
	}
	
	void Idle () {
		NavMeshAgent agent = GetComponentInParent<NavMeshAgent> ();
		if (!movingSound.isPlaying) {
			movingSound.Play();
		}

		//Debug.Log ("ROBOT IDLE");
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
			


		if (canSeePlayer()) {
			state = State.CHARGING;
			//Debug.Log("ROBOT GOING TO CHARGE!");
		}
	}
	
	public void Notify(Vector3 location)
	{
		//Debug.Log ("ROBOT NOTIFIED!");
		if (state == State.SLEEPING) {
			destination = location;
			state = State.ALERT;
		} else {
			if(!canSeePlayer())
			{
				destination = location;
				state = State.ALERT;
			}

		}


	}

	bool canSeePlayer  () 
	{
		Transform playerTransform = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ();
		Transform myTransform = GetComponentInParent<Transform> ();
		RaycastHit hit = new RaycastHit();
		Vector3 rayDirection = playerTransform.position - myTransform.position;
		if (Physics.Raycast(myTransform.position, rayDirection, out hit, 100)) {
			if (hit.transform == playerTransform) {
				//Debug.Log("I SEE THE PLAYER: ROBOT");
				return true;
			} else {
				return false;
			}

		}
		return false;
		

	}



	void OnTriggerEnter(Collider obj) {
		if (obj.tag.Equals ("Player")) {
			Application.LoadLevel("DeathScene");
		}
	}
	
	bool isClose(Vector3 first, Vector3 second)
	{
		if (Math.Abs(first.x - second.x) <= 5) 
		{
			if(Math.Abs(first.z - second.z) <=5)
			{
				return true;
			}
		}
		return false;
		
		
	}
}
