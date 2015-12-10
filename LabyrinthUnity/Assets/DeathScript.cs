using UnityEngine;
using System.Collections;

public class DeathScript : MonoBehaviour {

	bool firstTime;
	int waitCount;
	AudioSource Laughter;
	// Use this for initialization
	void Start () {

		firstTime = true;
		waitCount = 900;
 		Laughter = GetComponent<AudioSource> ();




	
	}
	
	// Update is called once per frame
	void Update () {

		if(firstTime) {
			firstTime = false;
			if (!Laughter.isPlaying) {
				Laughter.Play();
				
			}
		}
		else if(waitCount > 0){
			waitCount--;;
		}
		else
		{
			firstTime = true;
			Application.LoadLevel("Game");
		}




	
	}
}
