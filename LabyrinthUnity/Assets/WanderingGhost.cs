using UnityEngine;
using System.Collections;

public class WanderingGhost : MonoBehaviour {
	private Rigidbody body;
	private Rigidbody target;
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
	
	void OnTriggerEnter(Collider obj) {
		if (obj.tag.Equals ("Player")) {
			alert = true;
			turning = false;
			target = obj.attachedRigidbody;

			if(!scream.isPlaying) {
				scream.Play ();
			}
			scream.loop = true;
		}
	}
	
	void OnTriggerExit(Collider ob) {
		alert = false;
		target = null;
		scream.loop = false;
	}
	
	private void Alert() {
		body.angularVelocity = new Vector3 (0, 0, 0);
		body.velocity = new Vector3 (0, 0, 0);

		//Debug.DrawLine (target.position, body.position, Color.red);
		//Debug.DrawRay (body.position, body.transform.forward, Color.green);


		Vector3 desiredDirection = target.position - body.position;
		Vector2 desiredDirection2d = new Vector2(desiredDirection.x, desiredDirection.z).normalized;
		Vector2 bodyDirection2d = new Vector2 (body.transform.forward.x, body.transform.forward.z).normalized;

		float desiredAngle = Mathf.Atan2 (desiredDirection2d.y, desiredDirection2d.x);
		float bodyAngle = Mathf.Atan2 (bodyDirection2d.y, bodyDirection2d.x);

		float angle = (desiredAngle - bodyAngle) * Mathf.Rad2Deg;
		if (angle > 180) {
			angle -= 360;
		} else if (angle < -180) {
			angle += 360;
		}


		//Rotate to the direction of the Player
		if (angle > 2) {
			float rotation = -2;
			Turn (rotation);
		} 
		else if (angle < -2) {
			float rotation = 2;
			Turn (rotation);
		}
		else {
			//Move in that direction
			float wanderingSpeed = .7f;
			Forward (wanderingSpeed); 
		}
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
			float rotation = 1;
			Turn (rotation);
		} else {
			float wanderingSpeed = .4f;
			Forward (wanderingSpeed); 
		}
		wanderCount--;
	}
	
	private void Turn(float rotation) {
		body.angularVelocity = new Vector3 (0, rotation, 0);
	}
	
	private void Forward(float speed) {
		body.velocity = transform.forward * speed;
	}
}
