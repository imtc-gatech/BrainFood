using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class csLevelTimer : MonoBehaviour {

	public enum TimerState {
		Paused,
		Running
	}

	// Static initialization for now.
	float timeRemaining = 190.0f;
	TimerState timerState = TimerState.Paused;

	Text timerDisplay;

	void Awake () {
		timerDisplay = transform.FindChild ("TimerDisplay").GetComponent<Text> ();
	}

	void Start () {
		StartLevelTimer ();
	}
	
	// Update is called once per frame
	void Update () {
	
		if (timerState == TimerState.Running) {
			timeRemaining -= Time.deltaTime;

			if (timeRemaining <= 0.0f) {
				timerState = TimerState.Paused;
				timeRemaining = 0.0f;
				Debug.Log("Timer expired.");
			}

			DisplayTime();
		}
	}

	public void StartLevelTimer () {
		timerState = TimerState.Running;
	}

	// Update the time text display
	void DisplayTime () {
		string displayTime = string.Format ("{0:D1}:{1:D2}", Mathf.FloorToInt (timeRemaining / 60.0f), Mathf.FloorToInt (timeRemaining) % 60);
		timerDisplay.text = displayTime;

		if (timeRemaining < 5.0f) {
			timerDisplay.color = Color.red;
		} else if (timeRemaining < 15.0f) {
			timerDisplay.color = Color.yellow;
		}

	}

}
