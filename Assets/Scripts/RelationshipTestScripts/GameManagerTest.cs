using UnityEngine;
using System.Collections;

public class GameManagerTest : MonoBehaviour {

	public static GameManagerTest Instance = null;

	void Awake(){
		Instance = this;
	}


}
