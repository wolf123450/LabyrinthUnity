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
		destination = new Vector3();
		hearingScale = 0;//99; //If more difficult can be as low as 50
		loudNoiseHeard = false;
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
	
	public void addSound (Transform noiseSource, float volume) {
		/*
		Walking volume = 250
		Running volume = 1000
		Puddle volume (if we ever get to it) = 2250
		Chain volume = 4000
		Rock or bone volume = 6250

		This scale is only reasonable with a minotaur hearingScale = 50 to 99 
			where the lower the value, the more sensitive or difficult the minotaur is.
		*/
		//TODO: make the player walk slower... and maybe run a little slower too
		
		Debug.Log ("Method addSound() called");
		Vector3 position = GetComponentInParent<Transform> ().position;
		float distance = (noiseSource.position - position).magnitude;
		int power = (int) (volume / distance);
		if (power > hearingScale) {
			destination = noiseSource.position;
			Debug.Log("Noise heard");
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
		Debug.Log("I heard you: " + destination.ToString());
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
