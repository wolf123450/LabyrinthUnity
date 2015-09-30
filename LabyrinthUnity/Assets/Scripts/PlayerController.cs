using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float speed;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		float horizontal = Input.GetAxis ("Horizontal");
		float vertical = Input.GetAxis ("Vertical");

		GetComponent<Rigidbody>().velocity = new Vector3 (horizontal * speed, GetComponent<Rigidbody>().velocity.y, vertical * speed);
		if (horizontal == 0 && vertical == 0) {
			return;		
		}
		Quaternion q = new Quaternion ();
		q.SetLookRotation(new Vector3 (horizontal, 0, vertical));
		GetComponent<Rigidbody>().rotation = q;

	}
}
