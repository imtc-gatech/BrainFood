using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class csLocPassthrough : csLocation {

	// Represents a kitchen location that generates customer tickets periodically,
	// which can be filled by dropping the correct dish here.

	// TRANSFER IN: only same food as already contained.
	// TRANSFER OUT: never.

	private ParticleSystem celebration;
	private Image speechBubble;
	private Text speech;
	private Image urgentOrder;
	private bool isSpeaking = false;
	private csLevelManager levelManager;

	public override void Awake () {
		base.Awake ();
		celebration = transform.FindChild ("Celebration").GetComponent<ParticleSystem> ();
		speechBubble = transform.FindChild ("Canvas").FindChild("SpeechBubble").GetComponent<Image> ();
		speech = speechBubble.transform.FindChild ("Speech").GetComponent<Text> ();
		urgentOrder = transform.FindChild ("Canvas").FindChild("UrgentOrder").GetComponent<Image> ();
		speechBubble.gameObject.SetActive (false);
		urgentOrder.gameObject.SetActive (false);
		levelManager = Camera.main.GetComponent<csLevelManager> ();
	}

	void Start () {
		StartCoroutine (GenerateFood ());
	}
	
	IEnumerator GenerateFood () {
		// Possibly generate food each time the delay expires.
		while (true) {
			yield return new WaitForSeconds(Random.Range (3.0f, 20.0f));
			if (food == null) {
				MakeFood();
				// Prep time (here) represents customer patience.
				int patience = Random.Range(0,10);
				if (patience < 3) {
					// Impatient.
					prepTime = Random.Range (45.0f, 55.0f);
					urgentOrder.gameObject.SetActive(true);
				} else if (patience < 7) {
					// Normal.
					prepTime = Random.Range (60.0f, 70.0f);
					urgentOrder.gameObject.SetActive(false);
				} else {
					// Patient.
					prepTime = Random.Range (80.0f, 90.0f);
					urgentOrder.gameObject.SetActive(false);
				}
				StartFoodPrep(food);
			}
		}
	}

	public override void PrepComplete () {
		// Customer patience exhausted.
		StopActivity ();
		Destroy (food.gameObject, 0.1f);
		food = null;
		StartCoroutine (CustomerSays (RandomOutrage ()));
		Camera.main.GetComponent<csLevelManager> ().CustomerLeft ();
	}

	void MakeFood () {
		// Creates a new food object.  This serves as the ticket placeholder only.
		// It is not "real" food.  Null parameter makes a random food item.
		food = csFood.MakeFood(null, transform.position, levelManager.levelNumber);
	}

	string RandomThanks () {
		switch (Random.Range(0,4)) {
		case 0: return "Thanks!";
		case 1: return "Sweet!";
		case 2: return "Awesome!";
		case 3: return "Looks great!";
		}
		return "Whoa.";
	}

	string RandomOutrage () {
		switch (Random.Range(0,4)) {
		case 0: return "Forget it!";
		case 1: return "I'm leaving!";
		case 2: return "This is ridiculous!";
		case 3: return "Keep your food!";
		}
		return "Wharrrgarble!";
	}

	string GetWrongFoodComment (csFood offer) {
		if (offer.foodCookState == csFood.FoodCookState.Ruined) {
			return "That's ruined!";
		}
		if (offer.foodCookState == csFood.FoodCookState.Raw) {
			return "Raw food???";
		}
		if (food.foodType != offer.foodType) {
			return "I ordered " + food.foodType.ToString() + "!";
		}
		if ((offer.foodBatterState == csFood.FoodBatterState.Battered) && (food.foodBatterState == csFood.FoodBatterState.Unbattered)) {
			return "I ordered GRILLED " + food.foodType.ToString() + "!";
		}
		if ((offer.foodBatterState == csFood.FoodBatterState.Unbattered) && (food.foodBatterState == csFood.FoodBatterState.Battered)) {
			return "I ordered FRIED " + food.foodType.ToString() + "!";
		}
		if ((offer.sauceType == csFood.SauceType.None) && (food.sauceType != csFood.SauceType.None)) {
			return "Where's my sauce?";
		}
		if ((offer.sauceType != csFood.SauceType.None) && (food.sauceType == csFood.SauceType.None)) {
			return "Yuck, no sauce please!";
		}
		if (offer.sauceType != food.sauceType) {
			return "Ew, wrong sauce!";
		}
		// Should not reach this...
		return "I'm just picky!";
	}

	IEnumerator CustomerSays (string toSay) {
		// Ignore new speech if text is already displayed.
		if (isSpeaking) {
			yield break;
		}
		isSpeaking = true;
		speechBubble.gameObject.SetActive (true);
		speech.text = toSay;
		yield return new WaitForSeconds (1.0f);
		speechBubble.gameObject.SetActive (false);
		isSpeaking = false;
	}

	public override bool ContainsFood () {
		// We never really "contain" food.
		return false;
	}
	
	public override bool WillTakeFood (csFood offer) {
		if (food != null) {
			if (food.IsSameFood(offer)) {
				return true;
			} else {
				StartCoroutine(CustomerSays(GetWrongFoodComment(offer)));
			}
		}
		return false;
	}

	public override void TakeFood (csFood offer) {
		// TODO: credit points and whatnot here.
		StopActivity ();
		Destroy (offer.gameObject, 0.1f);
		Destroy (food.gameObject, 0.1f);
		food = null;
		celebration.Play ();
		Camera.main.GetComponent<csLevelManager> ().Celebrate ();
		StartCoroutine (CustomerSays (RandomThanks ()));
	}

	public override void StopActivity () {
		base.StopActivity ();
		urgentOrder.gameObject.SetActive (false);
	}

}
