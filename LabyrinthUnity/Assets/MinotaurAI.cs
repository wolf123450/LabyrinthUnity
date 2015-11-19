using UnityEngine;
using System.Collections;

public class MinotaurAI : MonoBehaviour {
	private enum State { SLEEPING, ALERT, WANDERING, CHARGING, RETURNING }
	private State state;
	private bool loudNoiseHeard;
	private Vector3 destination;
	private Vector3 layerLocation;
	private int hearingLimit;

	// Use this for initialization
	void Start () {
		destination = new Vector3();
		hearingLimit = 0;
		loudNoiseHeard = false;
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {
			case State.SLEEPING: Sleeping(); break;
			case State.ALERT: Alert(); break;
			case State.WANDERING: Wandering(); break;
			case State.CHARGING: Charging(); break;
			case State.RETURNING: Returning(); break;
		}
	}

	public void addSound (AudioSource sound, Rigidbody noiseSource) {
		if (sound.volume > hearingLimit) {
			destination = noiseSource.position;
			loudNoiseHeard = true;
		}
	}

	void Sleeping () {
		if (loudNoiseHeard) {
			//stop snore loop
			state = State.ALERT;
		}
	}

	void Alert () {
		//move slowly toward destination

		//if (player found) { state = State.CHARGING; }
		//else if (at position && player not found) { state = State.WANDERING; }
	}

	void Wandering () {
		//randomly move to a few interesctions near destination

		//if (player found) { state = State.CHARGING; }
		//else if (wander time equals 0 && player not found) { state = State.RETURNING; }
	}

	void Charging () {
		//move quickly toward the player
	}

	void Returning () {
		//go back to layer

		//if (player found) { state = State.CHARGING; }
		//else if (position = layer && player not found) { state = State.SLEEPING; loudNoiseHeard = false; start snore loop }
	}
}
