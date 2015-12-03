using UnityEngine;
using System.Collections;

public class MinotaurAI : MonoBehaviour {
	private enum State { SLEEPING, ALERT, WANDERING, CHARGING, RETURNING }
	private State state;
	private bool loudNoiseHeard;
	private Vector3 destination;
	private Vector3 layerLocation;
	private int hearingScale;
	private float angularSpeed_fast;
	private float speed_fast;
	private float angularSpeed_slow;
	private float speed_slow;
	private bool playerSighted;


	void Start () {
		layerLocation = GetComponent<Transform> ().position;
		hearingScale = 0;//99; //If more difficult can be as low as 50
		loudNoiseHeard = false;

		angularSpeed_fast = 120f;
		speed_fast = 3.5f;
		angularSpeed_slow = 120f;
		speed_slow = 3.5f;

		state = State.SLEEPING;
		playerSighted = false;
	}

	void Update () {
		Vector3 position = GetComponent<Transform> ().position;
		Vector3 playerPosition = GameObject.FindGameObjectWithTag ("Player").GetComponent<Rigidbody>().position;
		
		//Debug.Log("Distance" + (playerPosition - position).magnitude); //range from 3 to 50 units
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
		}
		if (loudNoiseHeard) {
			state = State.ALERT;

			snore.Stop();
		}
	}
	
	void Alert () {
		NavMeshAgent agent = GetComponent<NavMeshAgent> ();

		if (agent.destination != destination) {
			agent.angularSpeed = angularSpeed_slow;
			agent.speed = speed_slow;
			agent.destination = destination;
		}

		if (playerSighted) { 
			state = State.CHARGING; 
		}
		else if (GetComponent<Transform>().position == agent.destination) { 
			state = State.WANDERING; 
		}
	}
	
	void Wandering () {
		//randomly move to a few interesctions near destination
		
		//if (player found) { state = State.CHARGING; }
		//else if (wander time equals 0 && player not found) { state = State.RETURNING; }
	}
	
	void Charging () {
		if (!playerSighted) { 
			state = State.WANDERING;
		} else {
			NavMeshAgent agent = GetComponent<NavMeshAgent> ();
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			Vector3 playerLocation = player.GetComponent<Transform>().position;

			if (agent.destination != playerLocation) {
				agent.angularSpeed = angularSpeed_fast;
				agent.speed = speed_fast;
				agent.destination = playerLocation;
			}
			playerSighted = false;
		}
	}
	
	void Returning () {
		NavMeshAgent agent = GetComponent<NavMeshAgent> ();

		if (agent.destination != layerLocation) {
			agent.angularSpeed = angularSpeed_slow;
			agent.speed = speed_slow;
			agent.destination = layerLocation;
		}
		
		if (playerSighted) { 
			state = State.CHARGING; 
		} else if (GetComponent<Transform> ().position == layerLocation) { 
			state = State.SLEEPING; 
			loudNoiseHeard = false; 
		}
	}
}
