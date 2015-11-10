using UnityEngine;
using System.Collections;

public class NewGameButton : MonoBehaviour {

	public void LoadNewGame(){
		Application.LoadLevel("Main");
	}
}
