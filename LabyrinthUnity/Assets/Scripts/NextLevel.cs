using UnityEngine;
using System.Collections;

public class NextLevel : MonoBehaviour {

	public string nextLevel;

	void OnTriggerEnter(Collider other){
		Application.LoadLevel (nextLevel);
	}
}
