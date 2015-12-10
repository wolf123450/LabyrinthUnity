using UnityEngine;
using System.Collections;

public class NextLevel : MonoBehaviour {

	public string nextLevel;
	//public GameObject DungeonGenerator;

	void OnTriggerEnter(Collider other){
		//DungeonGenerator dGen = DungeonGenerator.GetComponent<DungeonGenerator>();
		if (other.tag.Equals ("Player")) {
			DungeonGenerator.DungeonGen.width += 5;
			DungeonGenerator.DungeonGen.height += 5;

			Application.LoadLevel (nextLevel);
		}
	}
}
