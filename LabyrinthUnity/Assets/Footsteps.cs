using UnityEngine;
using System.Collections;

public class Footsteps : MonoBehaviour {
	public AudioClip[] footsteps;
	AudioSource source;

	public int volumeLevel;
	// Use this for initialization
	void Start () {
		source = GetComponent<AudioSource> (); //Third audiosource
	}
	
	// Update is called once per frame
	void Update () {
		if(GetComponent<Rigidbody>().velocity.magnitude > 4.0f){
			if (!source.isPlaying){
				int r =  Random.Range (0, footsteps.Length);
				source.clip = footsteps[r];
				Vector3 location = GetComponent<Transform>().position;
				GameObject[] minotaurs = GameObject.FindGameObjectsWithTag("Minotaur");
				for(int i = 0; i < minotaurs.Length; i++)
				{
					minotaurs[i].GetComponent<MinotaurAI>().addSound(location, volumeLevel);
				}
				source.Play();
			}
		}
	}
}
