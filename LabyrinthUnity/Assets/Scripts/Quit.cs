using UnityEngine;
using System.Collections;

public class Quit : MonoBehaviour {

	public void EndGame(){
		# if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#endif
		Application.Quit();
	}
}
