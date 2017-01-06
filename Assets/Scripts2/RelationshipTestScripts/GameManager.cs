using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager Instance = null;

	public delegate void TurnEndedDelegate();
	public TurnEndedDelegate turnEnded;

	public int currentDay = 0;

	public bool isDayPaused = false;

	void Awake(){
		Instance = this;
		turnEnded += IncrementDaysOnTurn;
	}

	public void ActivateProducationCycle(){
		InvokeRepeating("EndTurn", 0f, 1f);
	}
		
	public void EndTurn(){
		if (isDayPaused) {
			return;
		}
		turnEnded();
	}

	public void TogglePause(){
		isDayPaused = !isDayPaused;
	}

	void IncrementDaysOnTurn(){
		currentDay++;
		UserInterfaceManager.Instance.UpdateDayCounter(currentDay);
	}






}
