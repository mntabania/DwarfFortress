using UnityEngine;
using System.Collections;

public class Woodsman {

	public static CitizenUpgradeRequirements req;

	static Woodsman(){
		req = new CitizenUpgradeRequirements();
		req.resource.Add (new Resource(RESOURCE.LUMBER, 0));
		req.resource.Add (new Resource(RESOURCE.STONE, 0));
	}
}
