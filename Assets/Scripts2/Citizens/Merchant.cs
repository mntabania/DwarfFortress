using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Merchant : Job {

	internal CityTest destinationCity;
	internal List<Tile> pathToTargetCity;
	internal List<Resource> tradeGoods;

	internal HexTile currentTile = null;

	public Merchant(){
		this._jobType = JOB_TYPE.MERCHANT;
		this._residence = RESIDENCE.OUTSIDE;
		this.pathToTargetCity = null;
		this.destinationCity = null;
	}
}
