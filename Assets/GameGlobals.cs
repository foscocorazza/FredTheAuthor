using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameStatus {
	Started, Finished
}

public class GameGlobals : MonoBehaviour {

	public GameObject povertyLabel;
	public GameObject stressLabel;
	public GameObject restartCard;
	public GameObject instructionCard;
	public GameObject instructionPopup;
	public GameStatus gameStatus = GameStatus.Started;
	public int ScreenWidth = 800;

	private float stressValue;
	private float povertyValue;

	public void Start() {

		#if UNITY_STANDALONE || UNITY_WEBPLAYER
		if (Screen.fullScreen) {
			Screen.fullScreen = false;
		}
		int h = 800;
		foreach (Resolution res in Screen.resolutions) {
			h = Mathf.Max (h, res.height);
		}
		Screen.SetResolution (h*9/16, h, false);
		#endif

		gameStatus = GameStatus.Finished;
		restartCard.GetComponent<CardComponent> ().Show ();
		instructionCard.GetComponent<CardComponent> ().Show ();
		initResources ();
	}

	private void setNewPosition(GameObject obj, float value) {
		Vector3 pos = obj.GetComponent<RectTransform> ().anchoredPosition;
		obj.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (value * (ScreenWidth-40), pos.y, pos.z);
	}

	public void setStress(float value){ 
		value = ClampValue (value);
		stressValue = value;
		setNewPosition (stressLabel, value); 
	}

	public void setPoverty(float value){ 
		value = ClampValue (value);
		povertyValue = value;
		setNewPosition (povertyLabel, value);
	}

	public void addStress(float value){
		setStress (stressValue + value);
	}

	public void addPoverty(float value){
		setPoverty (povertyValue + value);
	}

	private float ClampValue(float value) {
		if (value < 0) value = 0;
		if (value >= 1) {
			value = 1;
			finishGame();
		}
		return value;
	}

	private void finishGame() {
		restartCard.GetComponentInChildren<Text> ().text = "Restart!";
		gameStatus = GameStatus.Finished;
		restartCard.GetComponent<CardComponent> ().Show ();
		instructionCard.GetComponent<CardComponent> ().Show ();
	}

	public void Restart(){
		gameStatus = GameStatus.Started;
		initResources ();
		GetComponent<TimerScript> ().Restart ();
		GetComponent<SpawnCards>  ().Restart ();
	}

	public void TogglePopup(){
		Debug.Log ("pop");
		CardComponent c = instructionPopup.GetComponent<CardComponent> ();

		Debug.Log (c.status);
		if (c.status == CardStatus.Shown) {
			c.Hide ();
		} else if (c.status == CardStatus.Hidden) {
			c.Show ();
		}
	}

	private void initResources() {
		setStress (0.1f);
		setPoverty (0.7f);
	}
}
