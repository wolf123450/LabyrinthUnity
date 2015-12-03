using UnityEngine;
using System.Collections;

public class MinotaurAI : MonoBehaviour {
	private enum State { SLEEPING, ALERT, WANDERING, CHARGING, RETURNING }
	private State state;
	private bool loudNoiseHeard;
	private Vector3 destination;
	private Vector3 layerLocation;
	private int hearingScale;
	
	// Use this for initialization
	void Start () {
		hearingScale = 99; //If more difficult can be as low as 50
		loudNoiseHeard = false;
		state = State.SLEEPING;
	}
	
	// Update is called once per frame
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
	
	public void addSound (Transform noiseSource, string tag) {
		int volume = 10;
		switch (tag) {
			case "NoiseVolume1": volume = 250; break;
			case "NoiseVolume2": volume = 1000; break;
			case "NoiseVolume3": volume = 2250; break;
			case "NoiseVolume4": volume = 4000; break;
			case "NoiseVolume5": volume = 6250; break;
			/*
				This scale is only reasonable with a Minotaur hearingScale of 50 to 99 
					where the lower the value, the more sensitive or difficult the minotaur is.
			*/
		}
		
		
		//TODO: make the player walk slower... and maybe run a little slower too
		
		Vector3 position = GetComponentInParent<Transform> ().position;
		float distance = (noiseSource.position - position).magnitude;
		int power = (int) (volume / distance);
		if (power > hearingScale) {
			destination = noiseSource.position;
			loudNoiseHeard = true;
		}
	}
	
	void Sleeping () {
		Debug.Log ("Sleeping");
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
		Debug.Log ("Alert");
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
