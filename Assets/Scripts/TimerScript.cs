using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class TimerScript : MonoBehaviour {

	public GameObject DateDisplay;
	public GameObject TimeDisplay;
	private Text DateText;
	private Text TimeText;
	private float startedAt;
	private float secondsSinceLastUpdate;
	private float secondsElapsed;
	private float UPDATE_FREQUENCY = 0.1f;
	private GameGlobals globals;

	// Use this for initialization
	void Start () {
		DateText = DateDisplay.GetComponent<Text> ();
		TimeText = TimeDisplay.GetComponent<Text> ();

		if (DateText == null || TimeText == null) {
			Assert.IsTrue (false);
		}
		Restart ();

		globals = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameGlobals> ();
	}

	public void Restart() {
		startedAt = Time.time;
		secondsSinceLastUpdate = UPDATE_FREQUENCY;
	}
	
	// Update is called once per frame
	void Update () {
		if (globals.gameStatus == GameStatus.Finished) {
			return; 
		}

		secondsSinceLastUpdate += Time.deltaTime;
		if(secondsSinceLastUpdate >= UPDATE_FREQUENCY) {
			secondsSinceLastUpdate -= UPDATE_FREQUENCY;
			secondsElapsed = Time.time - startedAt;

			// Hours
			int hours = getHour();
			int minutes = getMinutes ();

			string suffix = "a.m.";
			if (hours >= 12) {
				suffix = "p.m.";
				if (hours > 12) {
					hours -= 12;
				}
			}
				
			TimeText.text = hours.ToString().PadLeft(2, '0') + ":" + 
				minutes.ToString().PadLeft(2, '0') + " " + suffix;


			// Days
			DateText.text = "Day " + getDay().ToString();

			// Stress & Poverty
			float onePerSecond = UPDATE_FREQUENCY/100;
			float increaser = (getDay()-1)/3 + 1;
			globals.addPoverty(onePerSecond*increaser);
			globals.addStress(onePerSecond*increaser);
		}
	}


	public int getHour() {
		return ((int)Mathf.Floor (secondsElapsed)) % 24;
	}

	public int getMinutes() {
		int minutes = 0;
		float decimalPart = secondsElapsed % 1;

		if (decimalPart > 0.50f) {
			minutes = 30;
		}
		return minutes;
	}

	public int getDay() {
		return (int)Mathf.Floor((secondsElapsed)/24) + 1;
	}
}
