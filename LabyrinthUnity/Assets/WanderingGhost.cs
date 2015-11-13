using UnityEngine;
using System.Collections;

public class WanderingGhost : MonoBehaviour {
	Rigidbody body;

	// Use this for initialization
	void Start () {
		body = GetComponentInParent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		//add random rotation to body

		//if(not moving too fast)
		body.AddForce (transform.forward * 0.2f);
	}
}
