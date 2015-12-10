using UnityEngine;
using System.Collections;
using System;

public class MinotaurAI : MonoBehaviour {
	private enum State { SLEEPING, ALERT, WANDERING, CHARGING, RETURNING }
	private State state;
	private bool loudNoiseHeard;
	private Vector3 destination;
	private Vector3 lairLocation;
	private int hearingScale;
	private float angularSpeed_fast;
	private float speed_fast;
	private float angularSpeed_slow;
	private float speed_slow;
	private bool firstTime;
	private int waitCount;


	void Start () {

		lairLocation = GetComponent<Transform> ().position;
		hearingScale = 0;//99; //If more difficult can be as low as 50
		loudNoiseHeard = false;
		firstTime = true;

		angularSpeed_fast = 120f;
		speed_fast = 3f;
		angularSpeed_slow = 80f;
		speed_slow = 1f;

		state = State.SLEEPING;
	}

	void Update () {

		switch (state) {
			case State.SLEEPING: Sleeping(); break;
			case State.ALERT: Alert(); break;
			case State.WANDERING: Wandering(); break;
			case State.CHARGING: Charging(); break;
			case State.RETURNING: Returning(); break;
		}
	}
	
	public void addSound (Vector3 noisePosition, int level) {
		int volume = 10;
		if (level == 1) {
			volume = 250;
		} else if (level == 2) {
			volume = 1000;
		} else if (level == 3) {
			volume = 2250;
		} else if (level == 4) {
			volume = 4000;
		} else if (level == 5) {
			volume = 6250;
		}
		/*
			This scale is only reasonable with a Minotaur hearingScale of 50 to 99 
				where the lower the value, the more sensitive to noise the minotaur is.
		*/
		Debug.Log ("AddSound");
		Vector3 position = GetComponentInParent<Transform> ().position;
		float distance = (noisePosition - position).magnitude;
		int power = (int) (volume / distance);
		if (power > hearingScale) {
			destination = noisePosition;
			loudNoiseHeard = true;
		}
	}
	
	void Sleeping () {
		AudioSource snore = GetComponent<AudioSource>();
		if (!snore.isPlaying) {
			snore.time = 0;
			snore.Play();
			//Debug.Log ("Sleeping");
		}
		if (loudNoiseHeard) {
			state = State.ALERT;
			firstTime = true;
			snore.Stop();
		}
	}
	
	void Alert () {
		NavMeshAgent agent = GetComponent<NavMeshAgent> ();

		if (!agent.destination.Equals (destination)) {
			agent.angularSpeed = angularSpeed_slow;
			agent.speed = speed_slow;
			agent.destination = destination;
			//Debug.Log ("Alert");
		}

		//Debug.Log (GetComponent<Transform>().position.ToString() + " : " + destination.ToString() );
		if (canSeePlayer()) { 
			state = State.CHARGING; 
			destination = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ().position;
		}
		else if (isClose(GetComponent<Transform>().position, destination)) {  
			if(firstTime) {
				waitCount = 500;
				firstTime = false;
			}
			else if(waitCount > 0){
				waitCount--;
			}
			else
			{
				state = State.RETURNING;
				firstTime = true;
				destination = lairLocation;
			}
		}
	}
	
	void Wandering () {
		NavMeshAgent agent = GetComponent<NavMeshAgent> ();
		agent.SetDestination (destination);
		//randomly move to a few interesctions near destination
		if (canSeePlayer ()) {
			state = State.CHARGING;
			destination = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ().position;
		} else {
			state = State.ALERT;
		}
		//if (player found) { state = State.CHARGING; }
		//else if (wander time equals 0 && player not found) { state = State.RETURNING; }
	}
	
	void Charging () {
		NavMeshAgent agent = GetComponent<NavMeshAgent> ();
		agent.SetDestination (destination);
		if (canSeePlayer())
		{

			
			destination = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ().position;
			
			agent.angularSpeed = angularSpeed_fast;
			agent.speed = speed_fast;
			
		}
		
		else 
		
		{ 

				state = State.WANDERING;


		} 
	



	}
	
	void Returning () {
		NavMeshAgent agent = GetComponent<NavMeshAgent> ();
		agent.SetDestination (destination);

		if (firstTime) {
			agent.angularSpeed = angularSpeed_slow;
			agent.speed = speed_slow;
			agent.destination = lairLocation;
			firstTime = false;
		} else if (!destination.Equals (lairLocation)) {
			state = State.ALERT;
			firstTime = true;
		}


		if (canSeePlayer()) { 
			state = State.CHARGING; 
			destination = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ().position;
		} else if (isClose(GetComponent<Transform> ().position,lairLocation)) {
			Debug.Log("Go to sleep....");
			state = State.SLEEPING;
			loudNoiseHeard = false; 
		}
	}

	bool canSeePlayer  () 
	{
		Transform playerTransform = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ();
		Transform myTransform = GetComponent<Transform> ();
		RaycastHit hit = new RaycastHit();
		Vector3 rayDirection = playerTransform.position - myTransform.position;

		if (Physics.Raycast(myTransform.position, rayDirection, out hit, 100)) {
			if (hit.transform == playerTransform) {
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
