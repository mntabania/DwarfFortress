using UnityEngine;
using System.Collections;

public class Pioneer : Job {

	public Pioneer(){
		this._jobType = JOB_TYPE.PIONEER;
		this._residence = RESIDENCE.INSIDE;
	}
}
