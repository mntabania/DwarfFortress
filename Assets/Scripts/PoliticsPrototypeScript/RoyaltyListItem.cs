using UnityEngine;
using System.Collections;

public class RoyaltyListItem : MonoBehaviour {

	[HideInInspector] public Royalty royalty;

	public Sprite maleSprite;
	public Sprite femaleSprite;

	public void ShowRoyaltyInfo(){
		PoliticsPrototypeUIManager.Instance.ShowRoyaltyInfo(royalty);
	}

	public void SetRoyalty(Royalty royalty){
		this.royalty = royalty;
		gameObject.GetComponentInChildren<UILabel>().text = royalty.name;
		if (royalty.gender == GENDER.FEMALE) {
			gameObject.GetComponentInChildren<UI2DSprite> ().sprite2D = femaleSprite;
			gameObject.GetComponentInChildren<UI2DSprite> ().width = 24;
			gameObject.GetComponentInChildren<UI2DSprite> ().height = 37;
		} else {
			gameObject.GetComponentInChildren<UI2DSprite> ().sprite2D = maleSprite;
			gameObject.GetComponentInChildren<UI2DSprite> ().width = 28;
			gameObject.GetComponentInChildren<UI2DSprite> ().height = 28;
		}
	}
}
