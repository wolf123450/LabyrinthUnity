using UnityEngine;
using System.Collections;

public class CameraAI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnTriggerEnter(Collider obj) {
		if (obj.tag.Equals ("Player")) {

		}
	}
}
