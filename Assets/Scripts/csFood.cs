using UnityEngine;
using System.Collections;

public class csFood : MonoBehaviour {

	public enum FoodType {
		Steak,
		Chicken,
		Shrimp,
		Broccoli,
		Potato,
		Pepper
	}

	public enum SauceType {
		None,
		Sauce1,
		Sauce2
	}

	public enum FoodCookState {
		Raw,
		Cooked,
		Ruined
	}

	public enum FoodChopState {
		Unchopped,
		Chopped
	}

	public enum FoodBatterState {
		Unbattered,
		Battered
	}

	public FoodType foodType = FoodType.Steak;
	public SauceType sauceType = SauceType.None;
	public FoodCookState foodCookState = FoodCookState.Raw;
	public FoodChopState foodChopState = FoodChopState.Unchopped;
	public FoodBatterState foodBatterState = FoodBatterState.Unbattered;

	private csSpriteManager sprites;
	private SpriteRenderer foodSprite;


	// Class method (factory).  Elegantly performs configurable setup here, rather than in calling script.
	public static csFood MakeFood (FoodType? foodType, Vector3 initialPosition, int levelNumber = 0) {
		csFood newFood = ((GameObject)Instantiate (Camera.main.GetComponent<csLevelManager> ().prefabFood)).GetComponent<csFood> ();
		if (foodType == null) {
			// Random valid dish.  Level number determines what is valid.  THIS IS A HACK TO GET A FEW TEMP LEVELS IN.
			newFood.foodCookState = FoodCookState.Cooked;
			newFood.sauceType = (SauceType)Random.Range (0, System.Enum.GetValues (typeof(SauceType)).Length);

			if (levelNumber == 0) {
				// Only grilled steak and broccoli, and sauces.
				if (Random.Range(0,2) == 0) {
					newFood.foodType = FoodType.Steak;
				} else {
					newFood.foodType = FoodType.Broccoli;
					newFood.foodChopState = FoodChopState.Chopped;
				}
			} else if (levelNumber == 1) {
				// Adds potato (grilled or french fries).
				int randfood = Random.Range(0,3);
				if (randfood == 0) {
					newFood.foodType = FoodType.Steak;
				} else if (randfood == 1) {
					newFood.foodType = FoodType.Broccoli;
					newFood.foodChopState = FoodChopState.Chopped;
				} else {
					newFood.foodType = FoodType.Potato;
					if (Random.Range(0,2) == 0) {
						newFood.foodChopState = FoodChopState.Chopped;
					}
				}
			} else if (levelNumber == 2) {
				// Adds batter (for steak) and red peppers.
				int randfood = Random.Range(0,4);
				if (randfood == 0) {
					newFood.foodType = FoodType.Steak;
					if (Random.Range(0,2) == 0) {
						newFood.foodBatterState = FoodBatterState.Battered;
					}
				} else if (randfood == 1) {
					newFood.foodType = FoodType.Broccoli;
					newFood.foodChopState = FoodChopState.Chopped;
				} else if (randfood == 2) {
					newFood.foodType = FoodType.Potato;
					if (Random.Range(0,2) == 0) {
						newFood.foodChopState = FoodChopState.Chopped;
					}
				} else {
					newFood.foodType = FoodType.Pepper;
				}
			} else {
				// Level number N (all options available as normal).
				newFood.foodType = (FoodType)Random.Range (0, System.Enum.GetValues (typeof(FoodType)).Length);
				if (newFood.foodType == FoodType.Broccoli) {
					newFood.foodChopState = FoodChopState.Chopped;
				}
				if (newFood.foodType == FoodType.Potato) {
					// Potatoes can be chopped (fries) or not (large grilled chunks).
					if (Random.Range(0,2) == 0) {
						newFood.foodChopState = FoodChopState.Chopped;
					}
				}
				if ((newFood.foodType == FoodType.Shrimp) || (newFood.foodType == FoodType.Steak) || (newFood.foodType == FoodType.Chicken)) {
					// Steak, shrimp, and chicken could be grilled or fried.
					if (Random.Range(0,2) == 0) {
						newFood.foodBatterState = FoodBatterState.Battered;
						// Chicken must be cut if it is to be fried.
						if (newFood.foodType == FoodType.Chicken) {
							newFood.foodChopState = FoodChopState.Chopped;
						}
					}
				}
			}
		} else {
			newFood.foodType = (FoodType) foodType;
		}

		newFood.transform.position = initialPosition;

		return (newFood);
	}


	void Awake () {

		sprites = Camera.main.GetComponent<csSpriteManager> ();

		foodSprite = transform.FindChild("FoodSprite").GetComponent<SpriteRenderer> ();

	}

	void Start () {
		// Wait until Start() to ensure sprite manager has created sprite dictionary.
		UpdateFoodSprite ();
	}
	
	public bool IsSameFood (csFood otherFood) {
		Debug.Log ("Was asked if same: " + PrintFood () + " and " + otherFood.PrintFood ());
		if ((foodType == otherFood.foodType) && (foodCookState == otherFood.foodCookState) && (sauceType == otherFood.sauceType) &&
		    (foodChopState == otherFood.foodChopState) && (foodBatterState == otherFood.foodBatterState)) {
			Debug.Log("Answered true");
			return true;
		}
		Debug.Log("Answered false");
		return false;
	}

	public string PrintFood () {
		return foodSprite.sprite.name;
	}

	void UpdateFoodSprite () {
		string name;
		if (foodCookState != FoodCookState.Ruined) {
			name = foodType.ToString () + '-' + foodCookState.ToString ();
			if (foodChopState == FoodChopState.Chopped) {
				name += '-' + foodChopState.ToString();
			}
			if (foodBatterState == FoodBatterState.Battered) {
				name += '-' + foodBatterState.ToString();
			}
			if (sauceType != SauceType.None) {
				name += '-' + sauceType.ToString();
			}
		} else {
			name = "AnyFood-Ruined";
		}
		Debug.Log("Loading sprite " + name);

		foodSprite.sprite = sprites.sprites [name];
	}

	public void SortForPlayer() {
		foodSprite.sortingLayerName = "RoomActors";
		foodSprite.sortingOrder = 1;

	}

	public void SortForRoom() {
		foodSprite.sortingLayerName = "RoomObjects";
		foodSprite.sortingOrder = 0;
	}

	public void SetCookState (csFood.FoodCookState newState) {
		foodCookState = newState;
		UpdateFoodSprite ();
	}

	public void SetChopState (csFood.FoodChopState newState) {
		foodChopState = newState;
		UpdateFoodSprite ();
	}

	public void SetBatterState (csFood.FoodBatterState newState) {
		foodBatterState = newState;
		UpdateFoodSprite ();
	}

	public void AddSauce (csFood.SauceType newSauce) {
		sauceType = newSauce;
		UpdateFoodSprite ();
	}

}
