using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public Camera playerCamera;
	public Vector3 offset;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		Vector3 newPos = offset + transform.position;
		playerCamera.transform.position = newPos;
	}
}
