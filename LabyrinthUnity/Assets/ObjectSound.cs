using UnityEngine;
using System.Collections;

public class ObjectSound : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnTriggerEnter(Collider obj) {
		if (obj.tag.Equals ("Player")) {
			Debug.Log("You hit me!");
			Transform location = GetComponentInParent<Transform>();
			GameObject[] minotaurs = GameObject.FindGameObjectsWithTag("Minotaur");
			for(int i = 0; i < minotaurs.Length; i++)
			{
				Debug.Log("Found a minotaur!");
				minotaurs[i].GetComponent<MinotaurAI>().addSound(location,250);
			}
		}
	}
}
