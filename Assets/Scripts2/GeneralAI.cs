using UnityEngine;
using System.Collections;

public class GeneralAI : MonoBehaviour {

	public delegate void Instruct(GENERAL_STATUS status, General general);
	public static event Instruct onInstruct;

	public delegate void Move();
	public static event Move onMove;

	public delegate void CheckTask();
	public static event CheckTask onCheckTask;

	public static void SendInstructions(GENERAL_STATUS status, General general){
		if(onInstruct != null){
			onInstruct (status, general);
		}
	}

	public static void TriggerMove(int currentDay){
		if(onMove != null){
			onMove ();
		}
	}

	public static void TriggerCheckTask(int currentDay){
		if(onCheckTask != null){
			onCheckTask ();
		}
	}
}
