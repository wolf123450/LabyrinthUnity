using UnityEngine;
using System.Collections;

public class WanderingGhost : MonoBehaviour {
	private Rigidbody body;
	private AudioSource scream;
	private bool alert;
	private int wanderCount;
	private bool turning;
	
	// Use this for initialization
	void Start () {
		body = GetComponentInParent<Rigidbody> ();
		scream = GetComponentInParent<AudioSource> ();
		alert = false;
		wanderCount = 0;
		turning = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (alert)
			Alert ();
		else
			Wander ();
	}
	
	void OnTriggerEnter() {
		alert = true;
		scream.loop = true;
		scream.Play ();
	}
	
	void OnTriggerExit() {
		alert = false;
		scream.loop = false;
	}
	
	private void Alert() {
		body.angularVelocity = new Vector3 (0, 0, 0);
		body.velocity = new Vector3 (0, 0, 0);
		
		//Rotate to the direction of the Player
		//Move in that direction
	}
	
	private void Wander() {
		bool outsideOfBounds = false;
		if (outsideOfBounds) {//check to see if out
			body.angularVelocity = new Vector3 (0, 0, 0);
			body.velocity = new Vector3 (0, 0, 0);
			wanderCount = 180;
			turning = true;
		}
		if (wanderCount <= 0) {
			if (turning) {
				//if outside of bounds, force wander 2000
				turning = false;
				body.angularVelocity = new Vector3 (0, 0, 0);
				wanderCount = Random.Range (300, 2000);
			} else {
				//if outside of bounds, force rotate 180
				turning = true;
				body.velocity = new Vector3 (0, 0, 0);
				wanderCount = Random.Range (20, 360);
			}
		}
		
		if (turning) {
			int rotation = 1;
			Turn (rotation);
		} else {
			float wanderingSpeed = .4f;
			Forward (wanderingSpeed); 
		}
		wanderCount--;
	}
	
	private void Turn(int rotation) {
		body.angularVelocity = new Vector3 (0, rotation, 0);
	}
	
	private void Forward(float speed) {
		body.velocity = transform.forward * speed;
	}
}
