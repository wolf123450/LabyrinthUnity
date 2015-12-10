using UnityEngine;
using System.Collections;

public class GameSetup : MonoBehaviour {

	// Use this for initialization
	void Start () {

		Cursor.visible = false;

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.Escape)) {

# if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#endif
			Application.Quit();
			//Debug.Log("Quit");
		}
		Cursor.lockState = CursorLockMode.Locked;
	
	}
}
