﻿using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;

public class PoliticsPrototypeUIManager : MonoBehaviour {

	public static PoliticsPrototypeUIManager Instance = null;

	public UILabel lblDate;
	[Space(10)]
	public GameObject kingdomListGO;
	public UILabel hideShowLbl;
	public GameObject kingdomListGrid;
	public GameObject kingdomListPrefab;
	[Space(10)]
	public GameObject kingdomInfoWindowGO;
	public UILabel lblKingdomName;
	public UILabel lblKingdomInfo;
	public UIGrid royaltyGrid;
	public GameObject royaltyPrefab;
	[Space(10)]
	public GameObject royaltyInfoWindowGO;
	public Transform fatherParent;
	public Transform motherParent;
	public Transform partnerParent;
	public UIGrid siblingsGrid;
	public UIGrid childrenGrid;
	public Transform currentRoyaltyParent;
	public UILabel lblCurrentLordInfo;
	[Space(10)]
	public UILabel lblWarTarget;
	public UIPopupList warDropdownMenu;

	public UILabel lblPeaceTarget;
	public UIPopupList peaceDropdownMenu;

	public UILabel lblNumOfCitiesToRevolt;

	private Royalty currentlySelectedRoyalty;
	private KingdomTest currentlySelectedKingdom;

	void Awake(){
		Instance = this;
	}

	public void LoadKingdoms(){
		KingdomListItem[] kingdoms = kingdomListGrid.GetComponentsInChildren<KingdomListItem>();
		for (int i = 0; i < kingdoms.Length; i++) {
			Destroy(kingdoms [i].gameObject);
		}

		for (int i = 0; i < PoliticsPrototypeManager.Instance.kingdoms.Count; i++) {
			GameObject kingdomGO = Instantiate(kingdomListPrefab, kingdomListGrid.transform) as GameObject;
			kingdomGO.transform.localPosition = Vector3.zero;
			kingdomGO.transform.localScale = Vector3.one;
			kingdomGO.GetComponentInChildren<UILabel>().text = PoliticsPrototypeManager.Instance.kingdoms[i].name;
			kingdomGO.GetComponent<KingdomListItem>().kingdom = PoliticsPrototypeManager.Instance.kingdoms[i];
		}
		kingdomListGrid.GetComponent<UIGrid>().enabled = true;
	}

	public void HideShowKingdoms(){
		if (kingdomListGO.transform.localPosition.x == 0f) {
			kingdomListGO.transform.localPosition = new Vector3(-225f, 0f, 0f);
			hideShowLbl.text = "[b]>>[/b]";
		} else {
			kingdomListGO.transform.localPosition = Vector3.zero;
			hideShowLbl.text = "[b]<<[/b]";
		}
	}

	public void ShowKingdomInfo(KingdomTest kingdom){
		currentlySelectedKingdom = kingdom;
		warDropdownMenu.Clear();
		peaceDropdownMenu.Clear();
		for (int i = 0; i < PoliticsPrototypeManager.Instance.kingdoms.Count; i++) {
			if (PoliticsPrototypeManager.Instance.kingdoms[i].kingdom.id != kingdom.id) {
				warDropdownMenu.AddItem(PoliticsPrototypeManager.Instance.kingdoms[i].kingdom.kingdomName);
			}
		}

		for (int i = 0; i < kingdom.kingdomsAtWarWith.Count; i++) {
			peaceDropdownMenu.AddItem(kingdom.kingdomsAtWarWith[i].kingdom.kingdomName);
		}

		lblKingdomInfo.text = "";
		lblKingdomName.text = kingdom.kingdomName + "\t # of cities: " + kingdom.cities.Count;
		lblKingdomInfo.text += "Current Lord: " + kingdom.assignedLord.name + "\n";
		lblKingdomInfo.text += "Next in line: \n";
		int succession = 5;
		if (kingdom.royaltyList.successionRoyalties.Count < succession) {
			succession = kingdom.royaltyList.successionRoyalties.Count;
		}

		for (int i = 0; i < succession; i++) {
			lblKingdomInfo.text += (i + 1).ToString () + ". " + kingdom.royaltyList.successionRoyalties [i].name + "\n";
		}

		lblKingdomInfo.text += "Kingdoms at war with: \n";

		for (int i = 0; i < kingdom.kingdomsAtWarWith.Count; i++) {
			lblKingdomInfo.text += (i+1).ToString() + ". " + kingdom.kingdomsAtWarWith[i].kingdom.kingdomName + "\n";
		}

		RoyaltyListItem[] children = royaltyGrid.GetComponentsInChildren<RoyaltyListItem>();
		for (int i = 0; i < children.Length; i++) {
			Destroy(children[i].gameObject);
		}

		for (int i = 0; i < kingdom.royaltyList.allRoyalties.Count; i++) {
			GameObject royaltyGO = Instantiate(royaltyPrefab, royaltyGrid.transform) as GameObject;
			royaltyGO.transform.localPosition = Vector3.zero;
			royaltyGO.transform.localScale = Vector3.one;
			royaltyGO.GetComponent<RoyaltyListItem>().SetRoyalty(kingdom.royaltyList.allRoyalties[i]);

		}
		royaltyGrid.GetComponent<UIGrid>().enabled = true;
		kingdomInfoWindowGO.SetActive(true);
		HideRoyaltyInfo();
	}

	public void HideKingdomInfo(){
		kingdomInfoWindowGO.SetActive(false);
	}

	public void ShowRoyaltyInfo(Royalty royalty){
		lblCurrentLordInfo.text = "";
		lblCurrentLordInfo.text += "Age: " + royalty.age.ToString () + "\t Gender: " + royalty.gender.ToString () + "\n";
		lblCurrentLordInfo.text += "Birthweek (WW/MM/YYYY): " + royalty.birthWeek.ToString() + "/" + royalty.birthMonth.ToString () + "/" + royalty.birthYear.ToString () + "\n";
		if (royalty.loyalLord != null) {
			lblCurrentLordInfo.text += "Loyal to: " + royalty.loyalLord.name + "\t";
		}
		if (royalty.hatedLord != null) {
			lblCurrentLordInfo.text += "Hates lord: " + royalty.hatedLord.name;
		}
		currentlySelectedRoyalty = royalty;
		HideKingdomInfo();
		List<RoyaltyListItem> objectsToDestroy = royaltyInfoWindowGO.GetComponentsInChildren<RoyaltyListItem>(true).ToList();
		for (int i = 0; i < objectsToDestroy.Count; i++) {
			Destroy (objectsToDestroy [i].gameObject);
		}

		GameObject currentRoyaltyGO = Instantiate(royaltyPrefab, currentRoyaltyParent) as GameObject;
		currentRoyaltyGO.transform.localPosition = Vector3.zero;
		currentRoyaltyGO.transform.localScale = Vector3.one;
		currentRoyaltyGO.GetComponent<RoyaltyListItem>().SetRoyalty(royalty);

		if (royalty.spouse != null) {
			currentRoyaltyGO = Instantiate (royaltyPrefab, partnerParent) as GameObject;
			currentRoyaltyGO.transform.localPosition = Vector3.zero;
			currentRoyaltyGO.transform.localScale = Vector3.one;
			currentRoyaltyGO.GetComponent<RoyaltyListItem> ().SetRoyalty (royalty.spouse);
		}

		if (royalty.father != null) {
			currentRoyaltyGO = Instantiate (royaltyPrefab, fatherParent) as GameObject;
			currentRoyaltyGO.transform.localPosition = Vector3.zero;
			currentRoyaltyGO.transform.localScale = Vector3.one;
			currentRoyaltyGO.GetComponent<RoyaltyListItem> ().SetRoyalty (royalty.father);
		}

		if (royalty.mother != null) {
			currentRoyaltyGO = Instantiate (royaltyPrefab, motherParent) as GameObject;
			currentRoyaltyGO.transform.localPosition = Vector3.zero;
			currentRoyaltyGO.transform.localScale = Vector3.one;
			currentRoyaltyGO.GetComponent<RoyaltyListItem> ().SetRoyalty (royalty.mother);
		}

		if (royalty.father != null) {
			for (int i = 0; i < royalty.father.children.Count; i++) {
				Royalty currentChild = royalty.father.children [i];
				if (currentChild.id == royalty.id) {
					continue;
				}
				currentRoyaltyGO = Instantiate (royaltyPrefab, siblingsGrid.transform) as GameObject;
				currentRoyaltyGO.transform.localPosition = Vector3.zero;
				currentRoyaltyGO.transform.localScale = Vector3.one;
				currentRoyaltyGO.GetComponent<RoyaltyListItem> ().SetRoyalty (currentChild);
			}
		}

		for (int i = 0; i < royalty.children.Count; i++) {
			currentRoyaltyGO = Instantiate(royaltyPrefab, childrenGrid.transform) as GameObject;
			currentRoyaltyGO.transform.localPosition = Vector3.zero;
			currentRoyaltyGO.transform.localScale = Vector3.one;
			currentRoyaltyGO.GetComponent<RoyaltyListItem>().SetRoyalty(royalty.children[i]);
		}
		siblingsGrid.enabled = true;
		childrenGrid.enabled = true;
		royaltyInfoWindowGO.SetActive(true);
	}

	public void HideRoyaltyInfo(){
		royaltyInfoWindowGO.SetActive(false);
	}

	public void PerformMarriage(){
		if (currentlySelectedRoyalty.gender == GENDER.MALE) {
			currentlySelectedRoyalty.AttemptToMarry();
			ShowRoyaltyInfo(currentlySelectedRoyalty);
		} else {
			Debug.Log ("Cannot trigger marriage on women");
		}
	}

	void UpdateDate(){
		lblDate.text = ((MONTH)PoliticsPrototypeManager.Instance.month).ToString() + " " + PoliticsPrototypeManager.Instance.year.ToString() + "\n Week# " 
			+ PoliticsPrototypeManager.Instance.week.ToString();
	}

	public void DeclareWar(){
		KingdomTest target = PoliticsPrototypeManager.Instance.GetKingdomByName (lblWarTarget.text);

		for (int i = 0; i < currentlySelectedKingdom.relationshipKingdoms.Count; i++) {
			if (currentlySelectedKingdom.relationshipKingdoms[i].kingdom.id == target.id) {
				currentlySelectedKingdom.relationshipKingdoms[i].isAtWar = true;
				break;
			}
		}

		for (int i = 0; i < target.relationshipKingdoms.Count; i++) {
			if (target.relationshipKingdoms[i].kingdom.id == currentlySelectedKingdom.id) {
				target.relationshipKingdoms[i].isAtWar = true;
				break;
			}
		}

		ShowKingdomInfo(currentlySelectedKingdom);
	}

	public void DeclarePeace(){
		KingdomTest target = PoliticsPrototypeManager.Instance.GetKingdomByName (lblPeaceTarget.text);

		for (int i = 0; i < currentlySelectedKingdom.relationshipKingdoms.Count; i++) {
			if (currentlySelectedKingdom.relationshipKingdoms[i].kingdom.id == target.id) {
				currentlySelectedKingdom.relationshipKingdoms[i].isAtWar = false;
				break;
			}
		}

		for (int i = 0; i < target.relationshipKingdoms.Count; i++) {
			if (target.relationshipKingdoms[i].kingdom.id == currentlySelectedKingdom.id) {
				target.relationshipKingdoms[i].isAtWar = false;
				break;
			}
		}

		ShowKingdomInfo(currentlySelectedKingdom);
	}

	public void BackToKingdomWindow(){
		ShowKingdomInfo (currentlySelectedKingdom);
	}

	public void StartRevolution(){
		int numOfCitiesToRevolt = Int32.Parse(lblNumOfCitiesToRevolt.text);
		if (numOfCitiesToRevolt > currentlySelectedKingdom.cities.Count) {
			Debug.LogWarning ("The kingdom only has " + currentlySelectedKingdom.cities.Count.ToString () + " cities.");
		} else {
			if (numOfCitiesToRevolt == currentlySelectedKingdom.cities.Count) {
				//Replace Lord
				currentlySelectedKingdom.AssignNewLord(currentlySelectedKingdom.royaltyList.successionRoyalties[0]);
				Debug.Log ("Revolution! Lord of: " + currentlySelectedKingdom.kingdomName + " was replaced");
			} else {
				List<CityTileTest> citiesForNewKingdom = new List<CityTileTest> ();
				for (int i = 0; i < numOfCitiesToRevolt; i++) {
					citiesForNewKingdom.Add (currentlySelectedKingdom.cities [i]);
				}

				currentlySelectedKingdom.RemoveCitiesFromKingdom(citiesForNewKingdom);
				KingdomTest newKingdom =  PoliticsPrototypeManager.Instance.CreateNewKingdom (citiesForNewKingdom);
				Debug.Log ("Revolution! New kingdom: " + newKingdom.kingdomName + " was created");
				LoadKingdoms();
			}

			ShowKingdomInfo(currentlySelectedKingdom);
		}
	}

	public void ToggleHighlightOfCities(){
		for (int i = 0; i < currentlySelectedKingdom.cities.Count; i++) {
			currentlySelectedKingdom.cities[i].hexTile.ToggleHighlight();
		}
	}

	void Update(){
		UpdateDate();
	}
}
