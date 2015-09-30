using UnityEngine;
using System.Collections;

public class RestartOnExit : MonoBehaviour {



	void OnTriggerExit(Collider other){
		Destroy (other.gameObject);
		Application.LoadLevel (Application.loadedLevel);
	}
}
