using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum CardStatus {Hidden, Shown, Hiding, Showing}
public enum CardSide {Left, Right}
public enum CardType {Normal, StartButton, InstructionButton, InstructionPopup}

public class CardComponent : MonoBehaviour, IPointerClickHandler {
	private const float ANIMATION_SECONDS = 1;

	private float animationStarted = 0;
	private Vector3 startingPosition;
	public CardStatus status = CardStatus.Hidden;
	public CardSide side;
	public float StressInfluence  = 0;
	public float PovertyInfluence = 0;
	public bool everlasting = false;
	public float MaxShownTime = 7f;
	public CardType type = CardType.Normal;

	private GameGlobals globals;

	public void Start() {
		startingPosition = transform.localPosition;
		globals = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameGlobals> ();
	}

	public void Update() {
		if (status  == CardStatus.Showing || status  == CardStatus.Hiding) {

			Vector3 start;
			Vector3 end;

			if (status == CardStatus.Showing) {
				float howMuch = type != CardType.InstructionPopup ? 250f : 620f;
				howMuch = side == CardSide.Left  ? howMuch : -howMuch;

				start = startingPosition;
				end = start + new Vector3 (howMuch, 0, 0);
			} else {
				start = transform.localPosition;
				end = startingPosition;
			}

			float t = Mathf.Pow((Time.time - animationStarted) / ANIMATION_SECONDS, 2);
			if (t > 1) {
				t = 1;
				status = status == CardStatus.Showing ? CardStatus.Shown : CardStatus.Hidden;
			}
			transform.localPosition = Berp (start, end, t);
		} else if (status == CardStatus.Shown) {
			float shownTime = (Time.time - animationStarted) - ANIMATION_SECONDS;
			if (shownTime > MaxShownTime && !everlasting) {
				Hide ();
			}
		}
	}

	public static float Berp(float start, float end, float value)
	{
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
		return start + (end - start) * value;
	}

	public static Vector3 Berp(Vector3 start, Vector3 end, float value)
	{
		return new Vector3(Berp(start.x, end.x, value), Berp(start.y, end.y, value), Berp(start.z, end.z, value));
	}


	public void OnPointerClick(PointerEventData eventData) 
	{
		if (status != CardStatus.Hiding && globals.gameStatus == GameStatus.Started) {
			Hide ();
			globals.addPoverty (PovertyInfluence);
			globals.addStress (StressInfluence);
		} else if (globals.gameStatus == GameStatus.Finished && status != CardStatus.Hiding && status != CardStatus.Hidden) {


			if (type == CardType.StartButton) {
				Hide ();
				globals.instructionCard.GetComponent<CardComponent> ().Hide ();
				globals.instructionPopup.GetComponent<CardComponent> ().Hide ();
				globals.Restart ();
			} else if (type == CardType.InstructionButton) {
				globals.TogglePopup ();
			} else if (type == CardType.InstructionPopup) {
				globals.TogglePopup ();
			}
		}
	}

	public void DrawAndShow(PresetCard card){
		Show (card.Text, card.Stress, card.Poverty, card.Color);
	}

	public void DrawAndShow(){
		 DrawAndShow(PresetCard.Draw ());
	}

	private void Show(string newText, float stress, float poverty, Color color) {
		GetComponentInChildren<Text>().text = newText;
		GetComponent<Image> ().color = color;
		StressInfluence = stress;
		PovertyInfluence = poverty;
		Show ();
	}

	public void Show() {
		status = CardStatus.Showing;
		animationStarted = Time.time;
	}
		
	public void Hide() {
		status = CardStatus.Hiding;
		animationStarted = Time.time;
	}
		
}


public class PresetCard {
	public static List<PresetCard> cards = new List<PresetCard> ();

	public string Text;
	public float Stress  = 0;
	public float Poverty = 0;
	public int Weight = 5;
	public Vector2Int WeightRange = Vector2Int.zero;

	public Color Color = Color.white;

	private static PresetCard generateCard(string text, int stress, int poverty) {
		PresetCard card = new PresetCard ();
		card.Text = text;
		card.Stress = stress/100f;
		card.Poverty = poverty/100f;
		if (stress > poverty) {
			card.Color = new Color (240f/255, 94f/255, 88f/255);
		} else if (stress < poverty) {
			card.Color = new Color (240f/255, 204f/255, 88f/255);
		}
		return card;
	}

	private static void addToCards(string text, int stress, int poverty, int weight) {
		PresetCard card = generateCard (text, stress, poverty);
		card.Weight = weight;

		int lastCard = cards.Count > 0 ? cards [cards.Count - 1].WeightRange.y : 0;
		card.WeightRange = new Vector2Int (lastCard, lastCard+weight);
		cards.Add (card);
	}

	private static void addToCards(string text, int stress, int poverty) {
		addToCards (text, stress, poverty, 5);
	}

	private static void initCards() {
		if (cards.Count > 0) return;

		// Stressful
		addToCards ("Write an article for 'Enough Feed'!", 10, -20);
		addToCards ("Write an article for 'The Merge'!", 10, -20);
		addToCards ("Write an article for 'Video Games United'!", 10, -20);
		addToCards ("Write an article on your blog!", 5, -10, 5+3);
		addToCards ("An offer for a new science fiction book!", 30, -50, 2);
		addToCards ("An offer for a new fantasy book!", 30, -50, 2);
		addToCards ("An offer for a new fantasy book trilogy!", 50, -75, 1);

		// Expensive
		addToCards ("Go out for a beer", -30, +20,4);
		addToCards ("Go out for a walk", -30, +10,4);
		addToCards ("Nap", -5, +5,4);
		addToCards ("Eat a snack", -5, +10,4);
		addToCards ("Read a book", -10, +10,4);
		addToCards ("Read an article", -5, +10,4);
		addToCards ("Watch a movie", -15, +15,4);
	}


	public static PresetCard FirstBookCard() {
		return generateCard ("An offer to publish your first book!", 20, -50);
	}

	public static PresetCard SleepCard() {
		return generateCard ("Sleep", -20, +15);
	}

	public static PresetCard MealCard() {
		return generateCard ("Have a nice meal", -15, +15);
	}

	public static PresetCard Draw(){
		initCards ();

		int MaxValue = cards.Count > 0 ? cards [cards.Count - 1].WeightRange.y : 0;
		int value = Random.Range (0, MaxValue);

		foreach (PresetCard card in cards) {
			if (card.WeightRange.x <= value && value < card.WeightRange.y) {
				return card;
			}
		}
		return null;
	}

}
