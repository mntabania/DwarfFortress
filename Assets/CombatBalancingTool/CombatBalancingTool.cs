using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CombatBalancingTool : MonoBehaviour {

	//Army 1
	public UILabel lblArmy1Count;
	public UILabel lblArmy1Lvl;
	public UILabel lblArmy1Type;
	public UILabel lblArmy1Race;
	public UILabel lblTotalArmy1Cost;
	public UILabel lblArmy1Units;
	public UILabel lblArmy1HP;
	public UILabel lblArmy1ATK;
	public UILabel lblArmy1HPTotal;
	public UILabel lblArmy1ATKTotal;


	//Army 2
	public UILabel lblArmy2Count;
	public UILabel lblArmy2Lvl;
	public UILabel lblArmy2Type;
	public UILabel lblArmy2Race;
	public UILabel lblTotalArmy2Cost;
	public UILabel lblArmy2Units;
	public UILabel lblArmy2HP;
	public UILabel lblArmy2ATK;
	public UILabel lblArmy2HPTotal;
	public UILabel lblArmy2ATKTotal;

	public UILabel battleLogs;

	public UIScrollView logsScrollView;

	public UIScrollBar scrollViewBar;

	enum ARMY_TYPE { DEFENSIVE, OFFENSIVE };

	int army1Count;
	int army2Count;

	int army1Lvl;
	int army2Lvl;

	ARMY_TYPE army1Type;
	ARMY_TYPE army2Type;

	RACE army1Race; 
	RACE army2Race;

	int army1Units;
	int army2Units;

	int army1HPPerUnit;
	int army2HPPerUnit;

	int army1ATK;
	int army2ATK;

	int totalArmy1HP;
	int totalArmy2HP;

	int totalArmy1ATK;
	int totalArmy2ATK;

	bool isSimulatingBattle = false;

	void Update(){
		if (isSimulatingBattle) {
			return;
		}

		if (lblArmy1Count.text == String.Empty || lblArmy2Count.text == String.Empty ||
		   lblArmy1Lvl.text == String.Empty || lblArmy2Lvl.text == String.Empty) {
			return;
		}

		lblTotalArmy1Cost.text = "[b]Army Cost:[/b] ";
		lblTotalArmy2Cost.text = "[b]Army Cost:[/b] ";
//		lblArmy1Units.text = "[b]Units:[/b] ";
//		lblArmy2Units.text = "[b]Units:[/b] ";

		army1Count = Int32.Parse(lblArmy1Count.text);
		army2Count = Int32.Parse(lblArmy2Count.text);

		army1Lvl = Int32.Parse(lblArmy1Lvl.text);
		army2Lvl = Int32.Parse(lblArmy2Lvl.text);

		army1Type = (ARMY_TYPE) Enum.Parse(typeof(ARMY_TYPE), lblArmy1Type.text);
		army2Type = (ARMY_TYPE) Enum.Parse(typeof(ARMY_TYPE), lblArmy2Type.text);

		army1Race = (RACE) Enum.Parse(typeof(RACE), lblArmy1Race.text); 
		army2Race = (RACE) Enum.Parse(typeof(RACE), lblArmy2Race.text);

		List<Resource> army1Cost = GetArmyCost(army1Race, army1Count, army1Lvl);
		List<Resource> army2Cost = GetArmyCost(army2Race, army2Count, army2Lvl);

		army1Units = GetUnitCount (army1Race, army1Count);
		army2Units = GetUnitCount (army2Race, army2Count);

		army1HPPerUnit = GetHPValue (army1Race, army1Lvl);
		army2HPPerUnit = GetHPValue (army2Race, army2Lvl);

		army1ATK = GetAttackValue (army1Race, army1Lvl);
		army2ATK = GetAttackValue (army2Race, army2Lvl);

		totalArmy1ATK = army1Units * GetAttackValue(army1Race, army1Lvl);
		totalArmy2ATK = army2Units * GetAttackValue(army2Race, army2Lvl);

		if (army1Type == ARMY_TYPE.DEFENSIVE) {
			totalArmy1HP = (int)((army1Units * army1HPPerUnit) * 1.25f);
		} else {
			totalArmy1HP = army1Units * army1HPPerUnit;
		}

		if (army2Type == ARMY_TYPE.DEFENSIVE) {
			totalArmy2HP = (int)((army2Units * army2HPPerUnit) * 1.25f);
		} else {
			totalArmy2HP = army2Units * army2HPPerUnit;
		}

		for (int i = 0; i < army1Cost.Count; i++) {
			lblTotalArmy1Cost.text += army1Cost[i].resourceType.ToString() + " " + army1Cost[i].resourceQuantity.ToString() + " ";
		}

		for (int i = 0; i < army2Cost.Count; i++) {
			lblTotalArmy2Cost.text += army2Cost[i].resourceType.ToString() + " " + army2Cost[i].resourceQuantity.ToString() + " ";
		}


		lblArmy1Units.text = "[b]Units:[/b] " + army1Units.ToString();
		lblArmy2Units.text = "[b]Units:[/b] " + army2Units.ToString();

		lblArmy1HP.text = "[b]HP per unit:[/b] " + army1HPPerUnit.ToString();
		lblArmy2HP.text = "[b]HP per unit:[/b] " + army2HPPerUnit.ToString();

		lblArmy1ATK.text = "[b]ATK per unit:[/b] " + army1ATK.ToString();
		lblArmy2ATK.text = "[b]ATK per unit:[/b] " + army2ATK.ToString();

		lblArmy1HPTotal.text = "[b]Total HP:[/b] " + totalArmy1HP.ToString();
		lblArmy2HPTotal.text = "[b]Total HP:[/b] " + totalArmy2HP.ToString();

		lblArmy1ATKTotal.text = "[b]Total ATK:[/b] " + totalArmy1ATK.ToString();
		lblArmy2ATKTotal.text = "[b]Total ATK:[/b] " + totalArmy2ATK.ToString();
	}

	public void SimulateBattle(){
		isSimulatingBattle = true;
		battleLogs.text = "";
		battleLogs.text += "Army 1 is: " + army1Type.ToString () + " and Army 2 is: " + army2Type.ToString () + "\n\n";
		int counter = 0;


		while (army1Units > 0 && army2Units > 0) {
			if (army1Type == ARMY_TYPE.DEFENSIVE && army2Type == ARMY_TYPE.DEFENSIVE) {
				battleLogs.text += "both armies are on defensive, battle cannot happen under these circumstances";
				isSimulatingBattle = false;
				return;
			} else {
				//Army 1 is attacking
				totalArmy1HP -= totalArmy2ATK;
				totalArmy2HP -= totalArmy1ATK;

				army1Units = (int)Math.Ceiling((double)(totalArmy1HP / army1HPPerUnit));
				army2Units = (int)Math.Ceiling((double)(totalArmy2HP / army2HPPerUnit));
				counter++;

				battleLogs.text += "Army 1 has " + army1Units.ToString() + " remaining units. \n\n";
				battleLogs.text += "Army 2 has " + army2Units.ToString() + " remaining units. \n\n";
			} 
		}

		if(army1Units <= 0 && army2Units <=0){
			battleLogs.text += "Both armies were wiped out.";
		} else if (army1Units > army2Units) {
			battleLogs.text += "Army 1 Won with remaining " + army1Units.ToString () + " units. It took " + counter + " rounds.";
		} else {
			battleLogs.text += "Army 2 Won with remaining " + army2Units.ToString () + " units. It took " + counter + " rounds.";
		}
		scrollViewBar.value = 1f;
		logsScrollView.UpdatePosition ();
		isSimulatingBattle = false;
	}



	int GetAttackValue(RACE race, int armyLvl){
		switch (race) {
		case RACE.HUMANS:
			return 30 + (4 * (armyLvl - 1));
		case RACE.ELVES:
			return 50 + (10 * (armyLvl - 1));
		case RACE.MINGONS:
			return 20 + (8 * (armyLvl - 1));
		case RACE.CROMADS:
			return 40 + (15 * (armyLvl - 1));
		}
		return 0;
	}

	int GetHPValue(RACE race, int armyLvl){
		switch (race) {
		case RACE.HUMANS:
			return 200 + (40 * (armyLvl - 1));
		case RACE.ELVES:
			return 200 + (20 * (armyLvl - 1));
		case RACE.MINGONS:
			return 160 + (20 * (armyLvl - 1));
		case RACE.CROMADS:
			return 400 + (60 * (armyLvl - 1));
		}
		return 0;
	}

	List<Resource> GetArmyCost(RACE race, int armyCount, int armyLvl){
		List<Resource> upgradeCost = new List<Resource>();
		int i, premiumResource = 0;
		switch (race) {
		case RACE.HUMANS:
			upgradeCost.Add (new Resource (RESOURCE.GOLD, 2000 + 200 * armyCount));
			upgradeCost.Add (new Resource (RESOURCE.STONE, 200 + 50 * armyCount));
			for (i = 1; i <= armyLvl; i++) {
				premiumResource += 100 * (i - 1);
			}
			upgradeCost.Add (new Resource (RESOURCE.METAL, premiumResource));
			return upgradeCost;
		case RACE.ELVES:
			upgradeCost.Add (new Resource (RESOURCE.GOLD, 2000 + 400 * armyCount));
			upgradeCost.Add (new Resource (RESOURCE.LUMBER, 200 + 50 * armyCount));
			for (i = 1; i <= armyLvl; i++) {
				premiumResource += 200 * (i - 1);
			}
			upgradeCost.Add (new Resource (RESOURCE.MANA, premiumResource));
			return upgradeCost;
		case RACE.MINGONS:
			upgradeCost.Add (new Resource (RESOURCE.GOLD, 2000 + 200 * armyCount));
			upgradeCost.Add (new Resource (RESOURCE.LUMBER, 200 + 25 * armyCount));
			for (i = 1; i <= armyLvl; i++) {
				premiumResource += 100 * (i - 1);
			}
			upgradeCost.Add (new Resource (RESOURCE.METAL, premiumResource));
			return upgradeCost;
		case RACE.CROMADS:
			upgradeCost.Add (new Resource (RESOURCE.GOLD, 2000 + 400 * armyCount));
			upgradeCost.Add (new Resource (RESOURCE.STONE, 200 + 150 * armyCount));
			for (i = 1; i <= armyLvl; i++) {
				premiumResource += 200 * (i - 1);
			}
			upgradeCost.Add (new Resource (RESOURCE.MANA, premiumResource));
			return upgradeCost;
		}
		return upgradeCost;
	}

	int GetUnitCount(RACE race, int armyCount){
		switch (race) {
		case RACE.HUMANS:
			return 100 + (20 * armyCount);
		case RACE.ELVES:
			return 100 + (20 * armyCount);
		case RACE.MINGONS:
			return 100 + (20 * armyCount);
		case RACE.CROMADS:
			return 100 + (20 * armyCount);
		}
		return 0;
	}

}
