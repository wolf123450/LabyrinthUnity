using UnityEngine;
using System.Collections;

public class DeathScript : MonoBehaviour {

	bool firstTime;
	int waitCount;
	AudioSource Laughter;
	// Use this for initialization
	void Start () {
		Cursor.visible = true;
		firstTime = true;
		waitCount = 300;
 		Laughter = GetComponent<AudioSource> ();
		Cursor.lockState = CursorLockMode.None;




	
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
			DungeonGenerator.DungeonGen.width = 5;
			DungeonGenerator.DungeonGen.height = 10;
			Application.LoadLevel("Game");
		}




	
	}
}
