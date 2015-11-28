using UnityEngine;
using System.Collections;

public class NextLevel : MonoBehaviour {

	public string nextLevel;
	//public GameObject DungeonGenerator;

	void OnTriggerEnter(Collider other){
		//DungeonGenerator dGen = DungeonGenerator.GetComponent<DungeonGenerator>();
		DungeonGenerator.DungeonGen.width += 5;
		DungeonGenerator.DungeonGen.height += 5;

		Application.LoadLevel (nextLevel);
	}
}
