using UnityEngine;
using System.Collections;

public class Hunter {

	public static CitizenUpgradeRequirements req;

	static Hunter(){
		req = new CitizenUpgradeRequirements();
		req.resource.Add (new Resource(RESOURCE.FOOD, 0));
		req.resource.Add (new Resource(RESOURCE.STONE, 0));
	}
}
