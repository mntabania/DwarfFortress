using UnityEngine;
using System.Collections;

public class CitizenAvatar : MonoBehaviour {

	[HideInInspector] public Citizen citizen;
	public GameObject goCitizenInfo;
	public TextMesh citizenInfo;

	void OnMouseOver(){
		UserInterfaceManager.Instance.ShowSpecificCitizenInfo (this.citizen);
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

}
