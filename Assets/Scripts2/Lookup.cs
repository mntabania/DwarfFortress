using UnityEngine;
using System.Collections;

public class Lookup {
	public static Roles[] ROLE_REF = {
		new Farmer(),
		new Hunter(),
		new Miner(),
		new Woodsman(),
		new Alchemist(),
//		new Farmer(),
//		new Grumpy(),
//		new Lion(),
//		new Sexy(),
//		new Emperor(),
//		new Bomb(),
//		new Evil(),
//		new Police(),
//		new Ice(),
//		new Hobo(),
//		new Gummy(),
//		new TimeCat(),
//		new SpaceCat(),
//		new WizardCat(),
//		new Dapper(),
//		new WarriorCat(),
//		new Flamboyant(),
//		new SuperheroCat(),
//		new Sphynx(),
//		new Alien(),
//		new Lion(),
//		new Zombie(),
//		new Monk(),
//		new FitnessInstructor(),

	};
	public static Roles GetRoleInfo(int roleId){
		Roles role = ROLE_REF [roleId];
		return role;
	}
}
