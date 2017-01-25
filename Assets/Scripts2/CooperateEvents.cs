using UnityEngine;
using System.Collections;

public class CooperateEvents {
	public int lord1Id;
	public DECISION lord1Decision;
	public int lord2Id;
	public DECISION lord2Decision;
	public LORD_EVENTS eventType;
	public int daysLeft;

	public CooperateEvents(int lord1Id, DECISION lord1Decision, int lord2Id, DECISION lord2Decision, LORD_EVENTS eventType, int daysLeft){
		this.lord1Id = lord1Id;
		this.lord1Decision = lord1Decision;
		this.lord2Id = lord2Id;
		this.lord2Decision = lord2Decision;
		this.eventType = eventType;
		this.daysLeft = daysLeft;
	}
}
