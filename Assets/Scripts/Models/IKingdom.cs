using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IKingdom {
//	List<CityTile> cities { get; set;}
//	RACE race { get; set;}
//	float populationGrowth { get; set;}
//	int cityPopulation { get; set;}
//	int altruism { get; set;}
//	int ambition { get; set;}
//	int performance { get; set;}
//	string kingdomName { get; set;}

	int[] GenerateTraits ();
	int GenerateArmyPopulation();
	int GetID ();
}
