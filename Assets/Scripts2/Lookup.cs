using UnityEngine;
using System.Collections;

public class Lookup {
	public static Job[] JOB_REF = {
		new Farmer(),
		new Hunter(),
		new Miner(),
		new Woodsman(),
		new Quarryman(),
		new Alchemist(),
//		new Brawler(),
//		new Archer(),
//		new Mage(),
	};
	public static Job GetJobInfo(int jobId){
		Job job = JOB_REF [jobId];
		return job;
	}
}
