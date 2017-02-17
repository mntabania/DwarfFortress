﻿using UnityEngine;
using System.Collections;

public class GeneralAI : MonoBehaviour {

	public delegate void Instruct(GENERAL_STATUS status, General general);
	public static event Instruct onInstruct;

	public delegate void Move();
	public static event Move onMove;

	public static void SendInstructions(GENERAL_STATUS status, General general){
		if(onInstruct != null){
			onInstruct (status, general);
		}
	}

	public static void TriggerMove(){
		if(onMove != null){
			onMove ();
		}
	}
}