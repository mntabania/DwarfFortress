using UnityEngine;
using System.Collections;

public class CitizenAvatar : MonoBehaviour {

	[HideInInspector] public Citizen citizen;
	[HideInInspector] public General general;
	public GameObject goCitizenInfo;
	public TextMesh citizenInfo;
	public Sprite[] avatars;

	void OnMouseOver(){
		if(citizen != null){
			UserInterfaceManager.Instance.ShowSpecificCitizenInfo (this.citizen);

		}
		if(general != null){
			

		}
//		citizenInfo.text = "";
//		goCitizenInfo.SetActive (true);
//		citizenInfo.text += "Name: " + citizen.name + "\n";
//		citizenInfo.text += "Job: " + citizen.job.jobType.ToString() + "\n";
//		citizenInfo.text += "Home City: " + citizen.city.cityName + "\n\n";
//
//		if (citizen.job.jobType == JOB_TYPE.MERCHANT) {
//			Merchant merchant = (Merchant)citizen.job;
//			if (merchant.targetCity != null) {
//				citizenInfo.text += "Target City: " + merchant.targetCity.cityName + "\n\n";
//			}
//
//			if(merchant.tradeGoods.Count > 0){
//				citizenInfo.text += "Trade Goods: \n";
//				for (int i = 0; i < merchant.tradeGoods.Count; i++) {
//					citizenInfo.text += merchant.tradeGoods[i].resourceQuantity.ToString() + " " + merchant.tradeGoods[i].resourceType.ToString() + "\n";
//				}
//			}
//		}
	}

	void OnMouseExit(){
		UserInterfaceManager.Instance.HideSpecificCitizenInfo();
	}

	internal void ProcessAvatar(){
		if(citizen != null){
			//TODO: change the fucking image
		}
		if(general != null){
			switch(general.city.kingdomTile.kingdom.kingdomRace){
			case RACE.HUMANS:
				this.GetComponent<SpriteRenderer> ().sprite = avatars [0];
				break;
			case RACE.ELVES:
				this.GetComponent<SpriteRenderer> ().sprite = avatars [0];
				break;
			case RACE.CROMADS:
				this.GetComponent<SpriteRenderer> ().sprite = avatars [0];
				break;
			case RACE.MINGONS:
				this.GetComponent<SpriteRenderer> ().sprite = avatars [0];
				break;
			}
		}
	}

	internal void MakeCitizenMove(HexTile startTile, HexTile targetTile){
		this.transform.position = Vector3.Lerp (startTile.transform.position, targetTile.transform.position, 0.5f);
	}

}
