using UnityEngine;
using System.Collections;

public class csLocFryer : csLocation {

	public override void Awake () {
		base.Awake();

		// Set prep and ruin time to override base location.  In code to allow upgrades.
		prepTime = 20.0f;
		ruinTime = 15.0f;
	}

	public override bool WillGiveFood () {
		return (ContainsFood () && (prepState != PrepState.Prepping));
	}
	
	public override csFood GiveFood () {
		csFood foodToReturn = food;
		food = null;
		StopActivity ();
		return foodToReturn;
	}

	public override bool WillTakeFood (csFood offer) {
		// Do not judge based on current occupancy.
		if (offer.foodCookState != csFood.FoodCookState.Raw) {
			// Raw food only.
			ShowError("Can't fry cooked food!");
			return false;
		}
		if (offer.foodType == csFood.FoodType.Potato) {
			if (offer.foodChopState == csFood.FoodChopState.Chopped) {
				// Chopped potatoes (fries) are fine.
				return true;
			} else {
				ShowError("Chop first!");
				return false;
			}
		}
		if (((offer.foodType == csFood.FoodType.Steak) || (offer.foodType == csFood.FoodType.Shrimp)) &&
			(offer.foodBatterState == csFood.FoodBatterState.Battered)) {
			// Battered steak or shrimp is fine.
			return true;
		}
		if ((offer.foodType == csFood.FoodType.Chicken) && (offer.foodChopState == csFood.FoodChopState.Chopped) &&
			(offer.foodBatterState == csFood.FoodBatterState.Battered)) {
			// Chopped, battered chicken (tenders) is fine.
			return true;
		}
		if (offer.foodBatterState != csFood.FoodBatterState.Battered) {
			ShowError ("Batter first!");
		}
		return false;
	}

	public override void TakeFood (csFood offer) {
		if (offer == null) return;
		TransformIn (offer);
		StartFoodPrep (offer);
		Debug.Log ("Fryer state is now: " + prepState);
	}

	public override void PrepComplete () {
		food.SetCookState (csFood.FoodCookState.Cooked);
		StartFoodRuining ();
	}

	public override void RuinComplete () {
		RuinFood ();
	}

}
