using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

	public void OnClickSpread(){
		KingdomGenerator.Instance.OnTurn ();
	}
}
