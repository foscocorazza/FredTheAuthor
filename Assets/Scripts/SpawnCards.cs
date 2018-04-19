using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnCards : MonoBehaviour {


	public GameObject[] cards;
	private float lastAction = 0;
	private float nextAction = 0;
	private GameGlobals globals;
	private bool firstCard = true;


	// Use this for initialization
	void Start () {
		globals = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameGlobals> ();
	}

	public void Restart() {
		firstCard = true;
		lastAction = 0;
		nextAction = 0;
	}
		
	void Update () {
		if (globals.gameStatus == GameStatus.Finished) {
			HideAll ();
			return;
		}

		float sinceLastAction = Time.time - lastAction;
		if (sinceLastAction > nextAction) {
			
			lastAction = Time.time;
			nextAction = getNextActionWait();

			if (AllShown ()) return;

			if (firstCard) {
				CardComponent card = cards [0].GetComponent<CardComponent> ();
				card.DrawAndShow (PresetCard.FirstBookCard());
				firstCard = false;
			} else {
				CardComponent card;
				do {
					card = cards [Random.Range (0, cards.Length)].GetComponent<CardComponent> ();
				} while (card.status != CardStatus.Hidden);

				card.DrawAndShow ();
			}
		}
	}

	bool AllShown() {
		foreach (GameObject obj in cards) {
			if (obj.GetComponent<CardComponent> ().status == CardStatus.Hidden) {
				return false;
			}
		}
		return true;
	}

	void HideAll() {
		foreach (GameObject obj in cards) {
			CardComponent card  = obj.GetComponent<CardComponent> ();
			if (card.status == CardStatus.Shown) {
				card.Hide ();
			}
		}
	}

	float getNextActionWait() {
		int day = globals.GetComponent<TimerScript> ().getDay ();
		float min = 4 * Mathf.Pow (3f, -0.25f * day);
		float max = 4 * Mathf.Pow (2f, -0.25f * day);
		return Random.Range (min, max);
	}

	// Detect swipe Left And Right
	/*
	void Update () {
		int horizontal = 0;
		int vertical = 0;

		//Check if we are running either in the Unity editor or in a standalone build.
		#if UNITY_STANDALONE || UNITY_WEBPLAYER
			
			horizontal = Input.GetButtonUp("Right") ? 1 : 0;
			horizontal = Input.GetButtonUp("Left") ? -1 : horizontal;

		//Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
		#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

		if (Input.touchCount > 0)
		{
			Touch myTouch = Input.touches[0];
			if (myTouch.phase == TouchPhase.Began) {
				touchOrigin = myTouch.position;
		    } else if(touchOrigin.x >= 0) {
				Vector2 touchEnd = myTouch.position;
				float x = touchEnd.x - touchOrigin.x;
				float y = touchEnd.y - touchOrigin.y;

			if (myTouch.phase == TouchPhase.Ended) {
				
				touchOrigin.x = -1;
				horizontal = x > 0 ? 1 : -1;
				vertical = y > 0 ? 1 : -1;
			} else {
			// Move card

			}
			}
		}

		#endif 

		if (horizontal != 0 || vertical != 0) {
			if (horizontal > 0) {
				card.GetComponent<Text> ().text = "Right";
			} else {
				card.GetComponent<Text> ().text = "Left";
			}
		} else {
			card.GetComponent<Text> ().text = "None";
		}
	
	}*/
}
